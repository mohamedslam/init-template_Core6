using Fab.Entities.Models.Communications;

namespace Fab.Infrastructure.Interfaces.Notifications;

/// <summary>
///     Транспорт для отправки уведомлений
/// </summary>
public interface INotificationTransport
{
    bool CanSend(Communication communication);

    Task SendAsync(Notification notification, Communication communication,
                   CancellationToken cancellationToken = default);

    Task SendBatchAsync(Notification notification, IReadOnlyCollection<Communication> communications,
                        CancellationToken cancellationToken = default);
}