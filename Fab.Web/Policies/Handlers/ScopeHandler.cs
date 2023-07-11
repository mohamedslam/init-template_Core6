using Fab.Entities.Abstractions.Interfaces;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Utils.Extensions;
using Fab.Web.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Fab.Web.Policies.Handlers;

public class ScopeHandler<TEntity> : AuthorizationHandler<ScopeRequirement<TEntity>>
    where TEntity : IEntity
{
    public sealed override Task HandleAsync(AuthorizationHandlerContext context) =>
        base.HandleAsync(context);

    protected sealed override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                                ScopeRequirement<TEntity> requirement)
    {
        requirement.Scope = await HandleScopeAsync(requirement.DbContext, default);
        context.Succeed(requirement);
    }

    protected virtual ValueTask<Expression<Func<TEntity, bool>>?> HandleScopeAsync(
        IReadonlyDbContext dbContext, CancellationToken cancellationToken) =>
        ValueTask.FromResult(HandleScope(dbContext));

    protected virtual Expression<Func<TEntity, bool>>? HandleScope(IReadonlyDbContext dbContext) =>
        throw new NotImplementedException();

    public Expression<Func<TEntity, bool>>? And(params Expression<Func<TEntity, bool>>?[] expressions) =>
        expressions.Aggregate(And);

    [return: NotNullIfNotNull("left")]
    [return: NotNullIfNotNull("right")]
    public Expression<Func<TEntity, bool>>? And(Expression<Func<TEntity, bool>>? left,
                                                Expression<Func<TEntity, bool>>? right) =>
        (left, right) switch
        {
            (not null, not null) => left.And(right),
            (not null, null) => left,
            (null, not null) => right,
            (null, null) => null
        };

    public Expression<Func<TEntity, bool>>? Or(params Expression<Func<TEntity, bool>>?[] expressions) =>
        expressions.Aggregate(Or);

    [return: NotNullIfNotNull("left")]
    [return: NotNullIfNotNull("right")]
    public Expression<Func<TEntity, bool>>? Or(Expression<Func<TEntity, bool>>? left,
                                               Expression<Func<TEntity, bool>>? right) =>
        (left, right) switch
        {
            (not null, not null) => left.Or(right),
            (not null, null) => left,
            (null, not null) => right,
            (null, null) => null
        };
}