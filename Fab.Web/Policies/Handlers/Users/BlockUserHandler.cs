using Fab.ApplicationServices.Interfaces;
using Fab.UseCases.Handlers.Users.Commands.BlockUser;
using Fab.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Fab.Web.Policies.Handlers.Users;

public class BlockUserHandler : AuthorizationHandler<OperationAuthorizationRequirement, BlockUserRequest>
{
    private readonly IContext _context;

    public BlockUserHandler(IContext context) =>
        _context = context;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   OperationAuthorizationRequirement requirement,
                                                   BlockUserRequest resource)
    {
        if (_context.UserId == resource.UserId)
        {
            context.Fail(this, "Нельзя блокировать самого себя");
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}