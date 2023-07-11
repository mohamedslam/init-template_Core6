using AltPoint.Filters.Definitions;
using Fab.Entities.Models.Customers;

namespace Fab.UseCases.Handlers.Customers.Configurations;

public class Filters : IFilterDefinition
{
    public void Register(IFilterCollectionBuilder builder)
    {
        builder.ForEntity<Customer>()
               .Property(x => x.Id)
               .Property(x => x.Label)
               .Property(x => x.CreatedAt)
               .Property(x => x.UpdatedAt)
               .Relation(x => x.Projects);
    }
    
}