using Fab.Utils.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Fab.Web.Extensions;

public static class AuthorizationExtensions
{
    public static void Fail(this AuthorizationHandlerContext context, IAuthorizationHandler handler, string message) =>
        context.Fail(new AuthorizationFailureReason(handler, message));

    public static void EnsureAuthorizationSucceeded(this AuthorizationResult result, bool mustExplicitFail = false)
    {
        if (!result.Succeeded && !mustExplicitFail || (result.Failure?.FailCalled ?? false))
        {
            var reasons = result.Failure
                                ?.FailureReasons
                                .ToList();

            var details = reasons?.Count > 0
                ? new AggregateException(reasons.Select(x =>
                    new RestException(x.Message, x.Handler.GetType().Name, HttpStatusCode.Forbidden)))
                : null;

            throw new RestException("У вас недостаточно прав для выполнения этого действия",
                HttpStatusCode.Forbidden, details);
        }
    }
}