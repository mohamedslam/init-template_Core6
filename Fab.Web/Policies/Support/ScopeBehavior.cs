using System.Reflection;
using Fab.Entities.Abstractions;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Support.Scopes;
using Fab.Utils.Extensions;
using Fab.Utils.Threading;
using Fab.Web.Extensions;
using Fab.Web.Policies.Requirements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Fab.Web.Policies.Support;

public class ScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuthorizationService _authorizationService;
    private readonly Func<IDbContext> _dbContextFactory;
    private readonly Func<IReadonlyDbContext> _readonlyDbContextFactory;

    public ScopeBehavior(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor,
                         Func<IDbContext> dbContextFactory, Func<IReadonlyDbContext> readonlyDbContextFactory)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
        _dbContextFactory = dbContextFactory;
        _readonlyDbContextFactory = readonlyDbContextFactory;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
                                        RequestHandlerDelegate<TResponse> next)
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            var entityType = typeof(TRequest).GetInterfaces()
                                             .FirstOrDefault(t => t.IsGenericType &&
                                                                  t.GetGenericTypeDefinition() ==
                                                                  typeof(IScopedRequest<>))
                                             ?.GenericTypeArguments
                                             .First();

            if (entityType != null)
            {
                await GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                               .First(x => x.Name == nameof(ConfigureScope) &&
                                           x.IsGenericMethod &&
                                           x.GetParameters().Length == 2)
                               .MakeGenericMethod(typeof(TRequest), entityType)
                               .Invoke(this, new object?[] { request, cancellationToken })!
                               .As<Task>();
            }
        }

        return await next();
    }

    private IReadonlyDbContext GetDbContextImplementation()
    {
        var ns = typeof(TRequest).Namespace!;
        var isCommand = ns.Contains(".Commands.");

        return isCommand
            ? _dbContextFactory().As<IReadonlyDbContext>()
            : _readonlyDbContextFactory();
    }

    private async Task ConfigureScope<TScopedRequest, TEntity>(TScopedRequest request,
                                                               CancellationToken cancellationToken)
        where TScopedRequest : IRequest<TResponse>, IScopedRequest<TEntity>
        where TEntity : IEntity
    {
        var scope = new ScopeRequirement<TEntity>(GetDbContextImplementation());

        var result = await _authorizationService.AuthorizeAsync(
                                                    _httpContextAccessor.HttpContext!.User,
                                                    new AuthorizationPolicyBuilder()
                                                        .AddRequirements(scope)
                                                        .Build())
                                                .WithCancellation(cancellationToken);

        result.EnsureAuthorizationSucceeded(mustExplicitFail: true);

        request.Scope = scope.Scope
                             ?.Let(x => new Spec<TEntity>(x));
    }
}