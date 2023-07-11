using AutoMapper;
using Fab.Infrastructure.Interfaces.Notifications;
using Fab.UseCases.Handlers.Notifications.Dto;
using Fab.Web.Hubs.Notifications.Protocol;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Fab.Web.Hubs.Notifications;

public class NotificationListener : INotificationHandler<NotificationDispatched>
{
    private readonly IHubContext<NotificationsHub, INotificationsClient> _hubContext;
    private readonly IMapper _mapper;

    public NotificationListener(IHubContext<NotificationsHub, INotificationsClient> hubContext, IMapper mapper)
    {
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task Handle(NotificationDispatched notification, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<NotificationDto>(notification.Contents);
        var userId = notification.Contents.ReceiverId
                                 .ToString();

        await _hubContext.Clients
                         .Group(userId)
                         .OnNotification(dto);
    }
}