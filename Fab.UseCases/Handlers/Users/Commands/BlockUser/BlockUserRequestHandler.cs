using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.BlockUser;

public class BlockUserRequestHandler : IRequestHandler<BlockUserRequest>
{
    private readonly IDbContext _dbContext;

    public BlockUserRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Unit> Handle(BlockUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
                                   .WithScope(request.Scope)
                                   .ById(request.UserId)
                                   .FirstOrDefaultAsync(cancellationToken)
                   ?? throw new NotFoundException("Пользователь не найден");

        user.IsBlocked = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}