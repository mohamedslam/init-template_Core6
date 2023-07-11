using Fab.Utils.Extensions;

namespace Fab.Infrastructure.Interfaces.Notifications;

public class Notification
{
    public string Subject { get; init; } = null!;
    public string Content { get; init; } = null!;

    public IReadOnlyDictionary<string, object> Extras { get; init; } = new Dictionary<string, object>();

    public T? GetExtra<T>(string key) =>
        Extras.ContainsKey(key)
            ? Extras[key].AsOrDefault<T>()
            : default;
}