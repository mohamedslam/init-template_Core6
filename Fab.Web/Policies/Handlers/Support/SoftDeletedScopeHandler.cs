using Fab.ApplicationServices.Interfaces;
using Fab.Entities.Abstractions;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Users;
using Fab.Infrastructure.DataAccess.Interfaces;
using System.Linq.Expressions;

namespace Fab.Web.Policies.Handlers.Support;

public abstract class SoftDeletedScopeHandler<T> : ScopeHandler<T>
    where T : IEntity, ISoftDeletes
{
    private readonly IContext _context;

    protected SoftDeletedScopeHandler(IContext context) =>
        _context = context;

    protected override Expression<Func<T, bool>>? HandleScope(IReadonlyDbContext dbContext) => null;

    protected override async ValueTask<Expression<Func<T, bool>>?> HandleScopeAsync(
        IReadonlyDbContext dbContext, CancellationToken cancellationToken) =>
        And(await base.HandleScopeAsync(dbContext, cancellationToken),
            DefaultSoftDeleteScope(dbContext)!);

    protected Spec<T>? DefaultSoftDeleteScope(IReadonlyDbContext dbContext) =>
        _context.Role switch
        {
            Role.Admin or Role.Manager =>
                null,

            Role.User =>
                new(x => x.DeletedAt == null),

            _ => throw new NotImplementedException()
        };
}