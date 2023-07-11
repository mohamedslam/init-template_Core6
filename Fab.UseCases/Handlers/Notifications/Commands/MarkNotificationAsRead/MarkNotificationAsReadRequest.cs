using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadRequest : IRequest
{
    [NotMapped]
    public Guid NotificationId { get; set; }
}