namespace Fab.Infrastructure.Interfaces.Notifications;

public interface INotificationRenderer<in TNotification, in TTransport>
    where TNotification : class
    where TTransport : INotificationTransport
{
    Task<Notification> RenderAsync(TNotification notification, TTransport transport);
}