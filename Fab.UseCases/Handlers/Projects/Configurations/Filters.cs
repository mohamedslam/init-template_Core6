using AltPoint.Filters.Definitions;
using Fab.Entities.Models.Projects;

namespace Fab.UseCases.Handlers.Projects.Configurations;

public class Filters : IFilterDefinition
{
    public void Register(IFilterCollectionBuilder builder)
    {
        builder.ForEntity<Project>()
               .Property(x => x.Id)
               .Property(x => x.Code)
               .Property(x => x.Label)
               .Relation(x => x.Customer).Ignore(x => x.CustomerId)
               .Relation(x => x.ModelIFC)
               .Property(x => x.Description)
               .Property(x => x.CreatedAt)
               .Property(x => x.UpdatedAt);

        builder.ForEntity<ProjectModel>()
            .Property(x => x.Id)
            .Property(x => x.Description)
            .Property(x => x.Label)
            .Property(x => x.CreatedAt)
            .Property(x => x.UpdatedAt);
    }
    
}