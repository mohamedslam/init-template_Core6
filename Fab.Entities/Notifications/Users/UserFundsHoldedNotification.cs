namespace Fab.Entities.Notifications.Users;

public class UserFundsHoldedNotification
{
    public Guid UserId { get; set; }
    public  Guid OrderId { get; set; }
    public decimal? Amount { get; set; }
}