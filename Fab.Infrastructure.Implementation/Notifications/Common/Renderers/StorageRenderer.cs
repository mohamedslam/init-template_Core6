using Fab.Infrastructure.Implementation.Notifications.Transports;
using Fab.Infrastructure.Interfaces.Notifications;
using SystemNotification = Fab.Entities.Models.Notifications.Notification;

namespace Fab.Infrastructure.Implementation.Notifications.Common.Renderers;

public abstract class StorageRenderer<T> : TextRenderer<T>,
                                           INotificationRenderer<T, StorageTransport>
                                                
    where T : class
{
    public abstract string EntityType { get; }
    public abstract string GetEntityId(T notification);

    public async Task<Notification> RenderAsync(T notification, StorageTransport transport) =>
        new()
        {
            Subject = Subject,
            Content = await RenderTextAsync(notification),
            Extras = new Dictionary<string, object>
            {
                [nameof(SystemNotification.EntityType)] = EntityType,
                [nameof(SystemNotification.EntityId)] = GetEntityId(notification)
            }
        };
}