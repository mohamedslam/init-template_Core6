using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.CreateUserCommunication;

public class CreateUserCommunicationRequestHandler : IRequestHandler<CreateUserCommunicationRequest, Guid>
{
    private readonly IDbContext _dbContext;
    private readonly ICommunicationService _communicationService;

    public CreateUserCommunicationRequestHandler(IDbContext dbContext, ICommunicationService communicationService)
    {
        _dbContext = dbContext;
        _communicationService = communicationService;
    }

    public async Task<Guid> Handle(CreateUserCommunicationRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
                                   .ById(request.UserId)
                                   .Include(x => x.Communications)
                                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        var communication =
            await _communicationService.FindOrCreateCommunicationAsync(request.Type, request.Value,
                cancellationToken);

        if (communication.Confirmed)
        {
            throw new ForbiddenException("Коммуникация уже занята");
        }

        var duplicate = user.Communications
                            .Where(x => x.DeviceId == null)
                            .FirstOrDefault(x => x.Type == communication.Type &&
                                                 x.Value == communication.Value);

        if (duplicate != null/* && communication.Type != CommunicationType.FirebasePushToken*/)
        {
            _dbContext.ChangeTracker.AcceptAllChanges();

            // await _communicationService.SendVerifyCodeAsync(duplicate, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return duplicate.Id;
        }

        communication.DeviceId = request.DeviceId;
        communication.User = user;

        // await _communicationService.SendVerifyCodeAsync(communication, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return communication.Id;
    }
}