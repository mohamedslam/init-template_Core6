using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.Implementation.Notifications.Common.Transports;
using Fab.Infrastructure.Interfaces.Notifications;
using Fab.Infrastructure.Interfaces.Push;

namespace Fab.Infrastructure.Implementation.Notifications.Transports;

public class PushTransport : BatchTransport
{
    private readonly IPushService _pushService;

    public PushTransport(IPushService pushService) =>
        _pushService = pushService;

    public override CommunicationType CommunicationType => CommunicationType.FirebasePushToken;

    public override Task SendBatchAsync(Notification notification, IReadOnlyCollection<Communication> communications,
                                        CancellationToken cancellationToken = default) =>
        _pushService.SendPushAsync(
            communications.Where(x => x.Type == CommunicationType.FirebasePushToken)
                          .Select(x => x.Value),
            new PushMessage
            {
                Title = notification.Subject,
                Body = notification.Content,
                ImageUrl = notification.GetExtra<Uri>(nameof(PushMessage.ImageUrl))?.AbsoluteUri,
                Extras = notification.GetExtra<Dictionary<string, string>>(nameof(PushMessage.Extras))
            }, cancellationToken);
}