using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.Implementation.Notifications.Common.Transports;
using Fab.Infrastructure.Interfaces.Email;
using Fab.Infrastructure.Interfaces.Notifications;

namespace Fab.Infrastructure.Implementation.Notifications.Transports;

public class EmailTransport : BatchTransport
{
    private readonly IEmailService _emailService;

    public EmailTransport(IEmailService emailService) => 
        _emailService = emailService;

    public override CommunicationType CommunicationType => CommunicationType.Email;

    public override Task SendBatchAsync(Notification notification, IReadOnlyCollection<Communication> communications,
                                        CancellationToken cancellationToken = default) =>
        _emailService.SendAsync(communications.Select(x => x.Value).ToList(),
            notification.Subject, notification.Content, cancellationToken);
}