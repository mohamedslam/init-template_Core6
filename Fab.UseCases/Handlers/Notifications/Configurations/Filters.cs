using AltPoint.Filters.Definitions;
using Fab.Entities.Models.Notifications;

namespace Fab.UseCases.Handlers.Notifications.Configurations;

public class Filters : IFilterDefinition
{
    public void Register(IFilterCollectionBuilder builder)
    {
        builder.ForEntity<Notification>()
               .Property(x => x.Id)
               .Relation(x => x.Receiver).Ignore(x => x.ReceiverId)
               .Property(x => x.EntityType)
               .Property(x => x.EntityId)
               .Property(x => x.Title)
               .Property(x => x.Description)
               .Property(x => x.ReadAt)
               .Property(x => x.CreatedAt)
               .Property(x => x.UpdatedAt);
    }
}