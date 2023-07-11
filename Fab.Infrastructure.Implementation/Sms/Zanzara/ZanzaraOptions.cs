using System.ComponentModel.DataAnnotations;

namespace Fab.Infrastructure.Implementation.Sms.Zanzara;

public class ZanzaraOptions
{
    [Required]
    public Uri Endpoint { get; set; } = null!;

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public string AppInfo { get; set; } = "fab";
}