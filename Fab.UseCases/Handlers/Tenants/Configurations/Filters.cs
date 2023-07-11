using AltPoint.Filters.Definitions;
using Fab.Entities.Models.Tenants;

namespace Fab.UseCases.Handlers.Tenants.Configurations;

public class Filters : IFilterDefinition
{
    public void Register(IFilterCollectionBuilder builder)
    {
        builder.ForEntity<Tenant>()
               .Property(x => x.Id)
               .Property(x => x.Label)
               .Property(x => x.CreatedAt)
               .Property(x => x.UpdatedAt);
    }
    
}