using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Abstractions;
using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Entities.Notifications.Users;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Notifications;
using Fab.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace Fab.ApplicationServices.Implementation.Communications;

public class CommunicationService : ICommunicationService
{
    private readonly IDbContext _dbContext;
    private readonly IVerificationCodeGenerator _verificationCodeGenerator;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CommunicationService> _logger;
    private readonly CommunicationOptions _options;

    public CommunicationService(IDbContext dbContext, INotificationService notificationService,
                                IVerificationCodeGenerator verificationCodeGenerator,
                                ILogger<CommunicationService> logger, IOptions<CommunicationOptions> options)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _notificationService = notificationService;
        _verificationCodeGenerator = verificationCodeGenerator;
        _logger = logger;
    }

    public Task<Communication> FindOrCreateCommunicationAsync(CommunicationType type, string value,
                                                              CancellationToken cancellationToken) =>
        FindOrCreateCommunicationAsync(new CommunicationShort(type, value), cancellationToken);

    public async Task<Communication> FindOrCreateCommunicationAsync(CommunicationShort communication,
                                                                    CancellationToken cancellationToken) =>
        (await FindOrCreateCommunicationsAsync(new[] { communication }, cancellationToken)).First();

    private static Spec<Communication> GetCommunicationSpecification(CommunicationShort s) =>
        Communication.ByTypeAndValue(s.Type, s.Value) & (x => x.Confirmed);

    public async Task<ICollection<Communication>> FindOrCreateCommunicationsAsync(
        ICollection<CommunicationShort>? shortCommunications, CancellationToken cancellationToken)
    {
        if (shortCommunications is not { Count: > 0 })
        {
            return new List<Communication>(0);
        }

        var condition = shortCommunications.Select(GetCommunicationSpecification)
                                           .Aggregate((acc, val) => acc | val);

        var communications = await _dbContext.Communications
                                             .Include(x => x.User)
                                             .Include(x => x.Verifications)
                                             .Where(x => x.DeviceId == null)
                                             .Where(condition)
                                             .ToListAsync(cancellationToken);

        foreach (var shortCommunication in shortCommunications.Where(c =>
                     communications.All(x => x.Type != c.Type &&
                                             x.Value != c.Value)))
        {
            var communication = new Communication
            {
                Type = shortCommunication.Type,
                Value = shortCommunication.Value
            };

            communications.Add(communication);
            _dbContext.Communications.Add(communication);
        }

        return communications;
    }

    public bool IsVerificationRequired(Communication communication) =>
        communication.Type switch
        {
            CommunicationType.FirebasePushToken => false,
            _ => true
        };

    public Task SendVerifyCodeAsync(Communication communication, CancellationToken cancellationToken)
    {
        var lastVerification = communication.Verifications
                                            .MaxBy(x => x.CreatedAt);

        if (lastVerification != null &&
            DateTime.UtcNow.Subtract(_options.VerificationTimeout) <= lastVerification.CreatedAt)
        {
            throw new RestException(
                $"Код подтверждения уже был отправлен. Повторите запрос через {_options.VerificationTimeout - (DateTime.UtcNow - lastVerification.CreatedAt)}",
                "RetryVerificationRequestLater",
                HttpStatusCode.BadRequest);
        }

        if (!IsVerificationRequired(communication))
        {
            return VerifyCommunicationAsync(communication, cancellationToken);
        }

        var staticCode = _options.StaticCodes
                                 .FirstOrDefault(x => x.Type == communication.Type &&
                                                      string.Equals(x.Value, communication.Value,
                                                          StringComparison.CurrentCultureIgnoreCase));

        var code = staticCode != null
            ? staticCode.Code
            : _verificationCodeGenerator.GenerateCode(communication.Type, communication.User);

        communication.Verifications = communication.Verifications
                                                   .Where(x => x.CreatedAt >
                                                               DateTime.UtcNow.Subtract(
                                                                   _options.VerificationTimeout))
                                                   .Append(new Verification
                                                   {
                                                       Code = code
                                                   })
                                                   .ToList();

        _dbContext.OnCommit(() =>
        {
            _notificationService.DispatchAsync(new ConfirmationCodeNotification
                                {
                                    Communication = communication.Value,
                                    Code = code
                                }, communication, CancellationToken.None)
                                .ContinueWith(task =>
                                {
                                    _logger.LogError(task.Exception,
                                        "An error occurred during sending notification");
                                }, TaskContinuationOptions.OnlyOnFaulted);
        });

        return Task.CompletedTask;
    }

    private bool IsDefaultCode(string code) =>
        !string.IsNullOrEmpty(_options.DefaultCode) &&
        _options.DefaultCode == code;

    public Task VerifyCommunicationAsync(Communication communication, string code,
                                         CancellationToken cancellationToken)
    {
        if (communication.Verifications.All(x => x.Code != code) && !IsDefaultCode(code))
        {
            throw new RestException("Неверный код подтверждения", HttpStatusCode.BadRequest);
        }

        return VerifyCommunicationAsync(communication, cancellationToken);
    }

    public async Task VerifyCommunicationAsync(Communication communication, CancellationToken cancellationToken)
    {
        if (!communication.Confirmed)
        {
            _dbContext.RemoveRange(
                await _dbContext.Communications
                                .Where(Communication.ByTypeAndValue(communication.Type, communication.Value))
                                .Where(x => x.Id != communication.Id &&
                                            !x.Confirmed &&
                                            x.UserId.HasValue)
                                .ToListAsync(cancellationToken));

            communication.Confirmed = true;
        }

        communication.Verifications.Clear();
    }
}