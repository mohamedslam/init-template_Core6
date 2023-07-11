namespace Fab.Infrastructure.Interfaces.Notifications;

[Flags]
public enum NotificationChannel
{
    Email = 1,
    Sms = 2,
    Push = 4,
    Storage = 8,
    Any = int.MaxValue, 
}