using Fab.Entities.Models.Communications;

namespace Fab.Infrastructure.Interfaces.Notifications;

public interface INotificationService
{
    Task DispatchAsync<TNotification>(TNotification notification, Communication communication,
                                      CancellationToken cancellationToken = default)
        where TNotification : class;

    Task DispatchAsync<TNotification>(TNotification notification, Communication communication,
                                      NotificationChannel channels = NotificationChannel.Any,
                                      CancellationToken cancellationToken = default)
        where TNotification : class;

    Task DispatchAsync<TNotification>(TNotification notification, IReadOnlyCollection<Communication> communications,
                                      CancellationToken cancellationToken = default)
        where TNotification : class;

    Task DispatchAsync<TNotification>(TNotification notification, INotificationGroup group,
                                      NotificationChannel channels = NotificationChannel.Any,
                                      CancellationToken cancellationToken = default)
        where TNotification : class;

    Task DispatchAsync<TNotification>(TNotification notification, IReadOnlyCollection<Communication> communications,
                                      NotificationChannel channels = NotificationChannel.Any,
                                      CancellationToken cancellationToken = default)
        where TNotification : class;
}