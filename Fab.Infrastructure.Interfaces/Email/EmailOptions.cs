namespace Fab.Infrastructure.Interfaces.Email;

public class EmailOptions
{
    public string SmtpHost { get; set; } = null!;
    public int SmtpPort { get; set; }

    public string Account { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string SenderEmail { get; set; } = null!;
    public string SenderName { get; set; } = null!;
}