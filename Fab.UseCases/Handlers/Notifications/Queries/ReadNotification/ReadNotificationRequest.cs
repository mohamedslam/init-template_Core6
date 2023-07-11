using System.ComponentModel.DataAnnotations.Schema;
using Fab.UseCases.Handlers.Notifications.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Notifications.Queries.ReadNotification;

public class ReadNotificationRequest : IRequest<NotificationDto>
{
    [NotMapped]
    public Guid NotificationId { get; set; }
}