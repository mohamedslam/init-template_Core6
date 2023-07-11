using Autofac;
using Fab.Utils.Threading;
using Fab.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Fab.Web.Policies.Support;

public class PolicyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationService _authorizationService;
    private readonly IComponentContext _component;

    public PolicyBehavior(IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService,
                          IComponentContext component)
    {
        _httpContextAccessor = httpContextAccessor;
        _authorizationService = authorizationService;
        _component = component;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
                                        RequestHandlerDelegate<TResponse> next)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null &&
            _component.IsRegistered(typeof(AuthorizationHandler<OperationAuthorizationRequirement, TRequest>)))
        {
            var result = await _authorizationService.AuthorizeAsync(
                                                        httpContext.User,
                                                        new AuthorizationPolicyBuilder()
                                                            .AddRequirements(new OperationAuthorizationRequirement
                                                            {
                                                                Name = typeof(TRequest).Name
                                                            })
                                                            .Build())
                                                    .WithCancellation(cancellationToken);

            result.EnsureAuthorizationSucceeded();
        }

        return await next();
    }
}