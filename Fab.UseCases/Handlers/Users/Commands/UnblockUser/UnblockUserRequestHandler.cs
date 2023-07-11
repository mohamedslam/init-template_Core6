using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.UnblockUser;

public class UnblockUserRequestHandler : IRequestHandler<UnblockUserRequest>
{
    private readonly IDbContext _dbContext;

    public UnblockUserRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Unit> Handle(UnblockUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
                                   .WithScope(request.Scope)
                                   .ById(request.UserId)
                                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        user.IsBlocked = false;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}