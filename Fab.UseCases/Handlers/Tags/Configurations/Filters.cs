using AltPoint.Filters.Definitions;
using Fab.Entities.Models.Tags;

namespace Fab.UseCases.Handlers.Tags.Configurations;

public class Filters : IFilterDefinition
{
    public void Register(IFilterCollectionBuilder builder)
    {
        builder.ForEntity<Tag>()
               .Property(x => x.Id)
               .Property(x => x.Label)
               .Property(x => x.CreatedAt)
               .Property(x => x.UpdatedAt);
    }
}