using Fab.Entities.Models.Users;
using Fab.Entities.Notifications.Users;
using Fab.Infrastructure.Implementation.Notifications.Common.Renderers;

namespace Fab.Infrastructure.Implementation.Notifications.Renderers.Users;

public class UserApprovedNotificationRenderer : StorageRenderer<UserApprovedNotification>
{
    public override string Subject => "Анкета одобрена";
    public override string EntityType => nameof(User);

    public override string GetEntityId(UserApprovedNotification notification) =>
        notification.UserId.ToString();

    public override string RenderText(UserApprovedNotification notification) =>
        "Ваша анкета прошла модерацию. Теперь Вы можете начать пользоваться сервисом!";
}