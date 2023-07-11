using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.ChangeUserRole;

public class ChangeUserRoleRequestHandler : IRequestHandler<ChangeUserRoleRequest>
{
    private readonly IDbContext _dbContext;

    public ChangeUserRoleRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Unit> Handle(ChangeUserRoleRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
                                   .WithScope(request.Scope)
                                   .ById(request.UserId)
                                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        user.Role = request.Role;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}