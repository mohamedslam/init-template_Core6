using Fab.UseCases.Handlers.Notifications.Commands.MarkAllNotificationsAsRead;
using Fab.UseCases.Handlers.Notifications.Commands.MarkNotificationAsRead;
using Fab.UseCases.Handlers.Notifications.Dto;
using Fab.UseCases.Handlers.Notifications.Queries.ListNotifications;
using Fab.UseCases.Handlers.Notifications.Queries.ReadNotification;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Fab.Web.Controllers;

/// <summary>
///     Уведомления
/// </summary>
[Route("v{version:apiVersion}/notifications")]
[ApiController, ApiVersion("1")]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator) =>
        _mediator = mediator;

    /// <summary>
    ///     Листинг уведомлений
    /// </summary>
    [HttpGet]
    public Task<Page<NotificationDto>> ListNotifications([FromQuery] ListNotificationsRequest request,
                                                         CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);

    /// <summary>
    ///     Чтение модели уведомления
    /// </summary>
    [HttpGet]
    [Route("{notificationId:guid}")]
    public Task<NotificationDto> ReadNotification([FromRoute] Guid notificationId,
                                                  CancellationToken cancellationToken) =>
        _mediator.Send(new ReadNotificationRequest
        {
            NotificationId = notificationId
        }, cancellationToken);

    /// <summary>
    ///     Отметить уведомление как прочитанное
    /// </summary>
    [HttpPost]
    [Route("{notificationId:guid}")]
    public Task MarkNotificationAsRead([FromRoute] Guid notificationId, CancellationToken cancellationToken) =>
        _mediator.Send(new MarkNotificationAsReadRequest
        {
            NotificationId = notificationId
        }, cancellationToken);

    /// <summary>
    ///     Отметить все уведомления как прочитанные
    /// </summary>
    [HttpPost]
    public Task MarkAllNotificationsAsRead(CancellationToken cancellationToken) =>
        _mediator.Send(new MarkAllNotificationsAsReadRequest(), cancellationToken);
}