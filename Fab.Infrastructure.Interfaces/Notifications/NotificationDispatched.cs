using MediatR;
using SystemNotification = Fab.Entities.Models.Notifications.Notification;

namespace Fab.Infrastructure.Interfaces.Notifications;

public class NotificationDispatched : INotification
{
    public SystemNotification Contents { get; }

    public NotificationDispatched(SystemNotification contents) =>
        Contents = contents;
}