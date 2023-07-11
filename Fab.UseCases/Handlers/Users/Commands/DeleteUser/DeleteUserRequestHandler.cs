using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.DeleteUser;

public class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest>
{
    private readonly IDbContext _dbContext;

    public DeleteUserRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Unit> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
                                   .WithScope(request.Scope)
                                   .ById(request.UserId)
                                   .Include(x => x.Communications)
                                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        foreach (var communication in user.Communications)
        {
            communication.Confirmed = false;
        }

        _dbContext.Remove(user);
        _dbContext.AttachRange(user.Communications); // to prevent detach communications (soft delete)
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}