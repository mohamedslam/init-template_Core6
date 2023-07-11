using System.Net;
using Fab.Utils.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Fab.Web.Policies.Support;

public class AuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _fallbackHandler = new();

    public Task HandleAsync(RequestDelegate next, HttpContext context,
                            AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
        {
            var reasons = authorizeResult.AuthorizationFailure
                                         ?.FailureReasons
                                         .ToList();

            var details = reasons?.Count > 0
                ? new AggregateException(reasons.Select(x =>
                    new RestException(x.Message, x.Handler.GetType().Name, HttpStatusCode.Forbidden)))
                : null;

            throw new RestException("У вас недостаточно прав для выполнения этого действия",
                HttpStatusCode.Forbidden, details);
        }

        return _fallbackHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}