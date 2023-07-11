namespace Fab.Infrastructure.Interfaces.Push;

public interface IPushService
{
    Task SendPushAsync(PushMessage message, CancellationToken cancellationToken = default);

    Task SendPushAsync(IEnumerable<string> pushTokens, PushMessage message,
                       CancellationToken cancellationToken = default);

    Task SendPushAsync(IReadOnlyCollection<PushMessage> messages, CancellationToken cancellationToken = default);
}