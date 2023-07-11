using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.Implementation.Notifications.Common.Transports;
using Fab.Infrastructure.Interfaces.Notifications;
using Fab.Infrastructure.Interfaces.Sms;

namespace Fab.Infrastructure.Implementation.Notifications.Transports;

public class SmsTransport : SimpleTransport
{
    private readonly ISmsService _smsService;

    public SmsTransport(ISmsService smsService) =>
        _smsService = smsService;

    public override CommunicationType CommunicationType => CommunicationType.Phone;

    public override Task SendAsync(Notification notification, Communication communication,
                                   CancellationToken cancellationToken = default) =>
        _smsService.SendAsync(communication.Value, notification.Content, cancellationToken);
}