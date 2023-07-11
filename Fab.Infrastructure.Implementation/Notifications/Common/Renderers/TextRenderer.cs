using Fab.Infrastructure.Implementation.Notifications.Transports;
using Fab.Infrastructure.Interfaces.Notifications;
using Fab.Infrastructure.Interfaces.Push;

namespace Fab.Infrastructure.Implementation.Notifications.Common.Renderers;

public abstract class TextRenderer<T> : INotificationRenderer<T, SmsTransport>,
                                        INotificationRenderer<T, PushTransport>
    where T : class
{
    public abstract string Subject { get; }

    public virtual string RenderText(T notification) =>
        throw new NotImplementedException();

    public virtual Task<string> RenderTextAsync(T notification) =>
        Task.FromResult(RenderText(notification));

    public virtual Task<Dictionary<string, string>> GetPushExtrasAsync(T notification) =>
        Task.FromResult(new Dictionary<string, string>(0));

    public async Task<Notification> RenderAsync(T notification, SmsTransport transport) =>
        new()
        {
            Subject = Subject,
            Content = await RenderTextAsync(notification),
        };

    public async Task<Notification> RenderAsync(T notification, PushTransport transport) =>
        new()
        {
            Subject = Subject,
            Content = await RenderTextAsync(notification),
            Extras = new Dictionary<string, object>
            {
                [nameof(PushMessage.Extras)] = await GetPushExtrasAsync(notification)
            }
        };
}