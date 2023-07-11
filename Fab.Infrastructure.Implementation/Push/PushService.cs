using Autofac.Features.OwnedInstances;
using Fab.Entities.Enums.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Push;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace Fab.Infrastructure.Implementation.Push;

public class PushService : IPushService
{
    private readonly Func<Owned<IDbContext>> _dbContextFactory;
    private readonly FirebaseMessaging _messaging;
    private readonly ILogger<PushService> _logger;

    private readonly AsyncPolicy _policy = Policy.Handle<FirebaseMessagingException>(e =>
                                                     e.MessagingErrorCode is
                                                         MessagingErrorCode.Unavailable or
                                                         MessagingErrorCode.Internal)
                                                 .WaitAndRetryAsync(6,
                                                     attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    public PushService(Func<Owned<IDbContext>> dbContextFactory, FirebaseMessaging messaging,
                       ILogger<PushService> logger)
    {
        _messaging = messaging;
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task SendPushAsync(PushMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var messageId = await _policy.ExecuteAsync(
                ct => _messaging.SendAsync(FromPushMessage(message), ct),
                cancellationToken);

            _logger.LogInformation(
                "Sent push (title={@Title}, body={@Body}) to recipient={PushToken}: messageId={MessageId}",
                message.Title, message.Body,
                message.PushToken,
                messageId);
        }
        catch (FirebaseMessagingException fme) when (fme.MessagingErrorCode == MessagingErrorCode.Unregistered)
        {
            await DeleteUnregisteredTokensAsync(message.PushToken!);
        }
    }

    public async Task SendPushAsync(IEnumerable<string> pushTokens, PushMessage message,
                                    CancellationToken cancellationToken = default)
    {
        var tokens = pushTokens as ICollection<string>
                     ?? pushTokens.ToList();

        var response = await _policy.ExecuteAsync(
            ct => _messaging.SendMulticastAsync(FromPushMessage(tokens, message), ct),
            cancellationToken);

        _logger.LogInformation(
            "Sent multicast push (title={@Title}, body={@Body}) to recipients={@PushTokens}: result={@Result}",
            message.Title, message.Body,
            pushTokens, response);
    }

    public async Task SendPushAsync(IReadOnlyCollection<PushMessage> messages,
                                    CancellationToken cancellationToken = default)
    {
        var messageList = messages as ICollection<PushMessage>
                          ?? messages.ToList();

        var response = await _policy.ExecuteAsync(
            ct => _messaging.SendAllAsync(messageList.Select(FromPushMessage), ct),
            cancellationToken);

        foreach (var message in messages)
        {
            _logger.LogInformation(
                "Sent push (title={@Title}, body={@Body}) to recipient={PushToken}",
                message.Title, message.Body,
                message.PushToken);
        }

        _logger.LogInformation("Totally sent {Count} pushes: result={@Result}", messages.Count, response);
    }

    private static Message FromPushMessage(PushMessage message) =>
        new()
        {
            Token = message.PushToken ?? throw new ArgumentNullException(
                nameof(PushMessage.PushToken), "PushToken not specified"),
            Notification = new Notification
            {
                Title = message.Title,
                Body = message.Body,
                ImageUrl = message.ImageUrl
            },
            Data = message.Extras
        };

    private static MulticastMessage FromPushMessage(IEnumerable<string> recipients, PushMessage message) =>
        new()
        {
            Tokens = recipients.ToList(),
            Notification = new Notification
            {
                Title = message.Title,
                Body = message.Body,
                ImageUrl = message.ImageUrl
            },
            Data = message.Extras
        };

    private async Task DeleteUnregisteredTokensAsync(params string[] tokens)
    {
        if (tokens is not { Length: > 0 })
        {
            return;
        }

        await using var scope = _dbContextFactory();
        var dbContext = scope.Value;

        var communications = await dbContext.Communications
                                            .Where(x => x.Type == CommunicationType.FirebasePushToken &&
                                                        tokens.Contains(x.Value))
                                            .ToListAsync();

        _logger.LogInformation(
            "Some push tokens are expired or unregistered from apps, so delete related communications: tokens={@Tokens}, communications={@Communications}",
            tokens, communications.Select(x => x.Id).ToArray());

        dbContext.RemoveRange(communications);
        await dbContext.SaveChangesAsync();
    }
}