using AltPoint.Filters.Definitions;
using Fab.Entities.Models.Resources;

namespace Fab.UseCases.Handlers.Resources.Configurations;

public class Filters : IFilterDefinition
{
    public void Register(IFilterCollectionBuilder builder)
    {
        builder.ForEntity<Resource>()
               .Property(x => x.Id)
               .Property(x => x.Name)
               .Ignore(x => x.Bucket)
               .Ignore(x => x.Target)
               .Ignore(x => x.OriginalName)
               .Relation(x => x.Creator).Ignore(x => x.CreatorId)
               .Property(x => x.Size)
               .Property(x => x.ContentType)
               .Property(x => x.Extension)
               .Ignore(x => x.Users)
               .Property(x => x.CreatedAt)
               .Property(x => x.UpdatedAt)
               .Property(x => x.DeletedAt);
    }
}