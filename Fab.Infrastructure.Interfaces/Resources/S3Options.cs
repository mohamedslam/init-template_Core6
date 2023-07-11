using System.ComponentModel.DataAnnotations;

namespace Fab.Infrastructure.Interfaces.Resources;

public class S3Options
{
    [Required]
    public string Endpoint { get; set; } = null!;

    [Required]
    public string Region { get; set; } = null!;

    [Required]
    public string Bucket { get; set; } = null!;

    [Required]
    public string AccessKey { get; set; } = null!;

    [Required]
    public string SecretKey { get; set; } = null!;

    [Required]
    public TimeSpan DownloadLinkExpiration { get; set; }
}