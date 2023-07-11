using Fab.ApplicationServices.Interfaces;
using Fab.Entities.Models.Resources;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Web.Policies.Handlers.Support;
using System.Linq.Expressions;

namespace Fab.Web.Policies.Handlers.Resources;

public class ResourceScopeHandler : SoftDeletedScopeHandler<Resource>
{
    private readonly IContext _context;

    public ResourceScopeHandler(IContext context) : base(context) =>
        _context = context;

    protected override Expression<Func<Resource, bool>>? HandleScope(IReadonlyDbContext dbContext) =>
        _context.Role switch
        {
            _ => base.HandleScope(dbContext)
        };
}