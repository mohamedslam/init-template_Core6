using Fab.Entities.Notifications.Users;
using Fab.Infrastructure.Implementation.Notifications.Common.Renderers;
using Fab.Infrastructure.Implementation.Notifications.Transports;
using Fab.Infrastructure.Interfaces.Notifications;
using RazorLight;

namespace Fab.Infrastructure.Implementation.Notifications.Renderers.Users;

public class ConfirmationCodeNotificationRenderer : TextRenderer<ConfirmationCodeNotification>,
                                                    INotificationRenderer<ConfirmationCodeNotification, EmailTransport>
{
    public override string Subject => "Подтвержение коммуникации";

    private readonly IRazorLightEngine _engine;

    public ConfirmationCodeNotificationRenderer(IRazorLightEngine engine) =>
        _engine = engine;

    public override string RenderText(ConfirmationCodeNotification notification) =>
        $"Код подтверждения: {notification.Code}";

    public async Task<Notification> RenderAsync(ConfirmationCodeNotification notification, EmailTransport transport) =>
        new()
        {
            Subject = Subject,
            Content = await _engine.CompileRenderAsync("ConfirmationCodeEmailTemplate", notification)
        };
}