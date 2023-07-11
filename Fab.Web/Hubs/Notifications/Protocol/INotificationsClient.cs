using Fab.UseCases.Handlers.Notifications.Dto;

namespace Fab.Web.Hubs.Notifications.Protocol;

public interface INotificationsClient
{
    Task OnNotification(NotificationDto notification);
}