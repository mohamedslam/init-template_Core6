namespace Fab.Entities.Notifications.Users;

public class ConfirmationResetPasswordCodeNotification
{
    public string Communication { get; set; } = null!;
    public string Code { get; set; } = null!;
}