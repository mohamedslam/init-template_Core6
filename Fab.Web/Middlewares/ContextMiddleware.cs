using Fab.ApplicationServices.Implementation;
using Fab.Entities.Enums.Users;
using Fab.Utils.Extensions;
using Sentry;
using Serilog.Context;
using System.Security.Claims;

namespace Fab.Web.Middlewares;

public class ContextMiddleware : IMiddleware
{
    private readonly Context _context;

    public ContextMiddleware(Context context) =>
        _context = context;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            _context.IsAuthenticated = true;
            _context.UserId = context.User.FindFirst("id")!
                                     .Value.Let(Guid.Parse);

            _context.Role = context.User.FindFirst(ClaimTypes.Role)!
                                   .Value.Let(Enum.Parse<Role>);

            var actors = context.User
                                .FindAll(ClaimTypes.Actor)
                                .ToList();

            SentrySdk.ConfigureScope(scope =>
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                scope.User ??= new User();
                scope.User.Id = _context.UserId.ToString();

                if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    scope.User.IpAddress = context.Request.Headers["X-Forwarded-For"];
                }
                else if (context.Request.Headers.ContainsKey("X-Real-IP"))
                {
                    scope.User.IpAddress = context.Request.Headers["X-Real-IP"];
                }
            });
        }

        using (LogContext.PushProperty("RequestId", context.TraceIdentifier))
        {
            await next(context);
        }
    }

    private static Guid GuidFromActorClaim(Claim actor) =>
        actor.Value
             .Split(':')
             .Last()
             .Let(Guid.Parse);
}