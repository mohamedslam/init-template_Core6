using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.DeleteUserCommunication;

public class DeleteUserCommunicationRequestHandler : IRequestHandler<DeleteUserCommunicationRequest>
{
    private readonly IDbContext _dbContext;

    public DeleteUserCommunicationRequestHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(DeleteUserCommunicationRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
                                   .Include(x => x.Communications)
                                   .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        var communication = user.Communications.FirstOrDefault(x => x.Id == request.CommunicationId)
                            ?? throw new NotFoundException("Коммуникация не найдена");

        if (communication.Confirmed &&
            user.Communications.Count(x => x.Confirmed) <= 1)
        {
            throw new ForbiddenException("Нельзя удалить все коммуникации");
        }

        _dbContext.Remove(communication);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}