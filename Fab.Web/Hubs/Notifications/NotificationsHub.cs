using Fab.Utils.Extensions;
using Fab.Web.Hubs.Notifications.Protocol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fab.Web.Hubs.Notifications;

[Authorize]
public class NotificationsHub : Hub<INotificationsClient>
{
    private Guid? UserId => Context.User
                                   ?.FindFirst("id")
                                   ?.Value
                                   .Let(Guid.Parse);

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, UserId.ToString()!);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId.ToString()!);
        await base.OnDisconnectedAsync(exception);
    }
}