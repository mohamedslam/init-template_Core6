using System.Linq.Expressions;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Infrastructure.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Fab.Web.Policies.Requirements;

public class ScopeRequirement<TEntity> : IAuthorizationRequirement
    where TEntity : IEntity
{
    public IReadonlyDbContext DbContext { get; }
    public Expression<Func<TEntity, bool>>? Scope { get; set; }

    public ScopeRequirement(IReadonlyDbContext dbContext) =>
        DbContext = dbContext;
}