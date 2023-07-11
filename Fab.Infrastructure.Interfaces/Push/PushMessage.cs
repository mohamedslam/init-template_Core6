namespace Fab.Infrastructure.Interfaces.Push;

public class PushMessage
{
    public string? PushToken { get; set; }

    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string? ImageUrl { get; set; }

    public Dictionary<string, string>? Extras { get; set; }
}