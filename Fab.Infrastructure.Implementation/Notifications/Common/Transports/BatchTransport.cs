using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.Interfaces.Notifications;

namespace Fab.Infrastructure.Implementation.Notifications.Common.Transports;

public abstract class BatchTransport : INotificationTransport
{
    public virtual CommunicationType CommunicationType => throw new NotImplementedException();

    public virtual bool CanSend(Communication communication) =>
        communication.Type == CommunicationType;

    public Task SendAsync(Notification notification, Communication communication,
                          CancellationToken cancellationToken = default) =>
        SendBatchAsync(notification, new []{ communication }, cancellationToken);

    public abstract Task SendBatchAsync(Notification notification, IReadOnlyCollection<Communication> communications,
                                        CancellationToken cancellationToken = default);
}