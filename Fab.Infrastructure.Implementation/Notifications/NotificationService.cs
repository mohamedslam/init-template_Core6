using Autofac;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Notifications;
using Fab.Utils.Extensions;
using Microsoft.Extensions.Logging;

namespace Fab.Infrastructure.Implementation.Notifications;

public class NotificationService : INotificationService
{
    private readonly ILifetimeScope _scope;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILifetimeScope scope, ILogger<NotificationService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    public Task DispatchAsync<TNotification>(TNotification notification, Communication communication,
                                             CancellationToken cancellationToken = default)
        where TNotification : class =>
        DispatchAsync(notification, communication, NotificationChannel.Any, cancellationToken);

    public Task DispatchAsync<TNotification>(TNotification notification, Communication communication,
                                             NotificationChannel channels = NotificationChannel.Any,
                                             CancellationToken cancellationToken = default)
        where TNotification : class =>
        DispatchAsync(notification, new[] { communication }, NotificationChannel.Any, cancellationToken);

    public Task DispatchAsync<TNotification>(TNotification notification,
                                             IReadOnlyCollection<Communication> communications,
                                             CancellationToken cancellationToken = default)
        where TNotification : class =>
        DispatchAsync(notification, communications, NotificationChannel.Any, cancellationToken);

    public async Task DispatchAsync<TNotification>(TNotification notification, INotificationGroup group,
                                                   NotificationChannel channels = NotificationChannel.Any,
                                                   CancellationToken cancellationToken = default)
        where TNotification : class
    {
        var dbContext = _scope.Resolve<IReadonlyDbContext>();
        var communications = await group.ResolveRecipientsAsync(dbContext, cancellationToken);
        await DispatchAsync(notification, communications.DistinctBy(x => x.Id).ToList(), channels, cancellationToken);
    }

    public async Task DispatchAsync<TNotification>(TNotification notification,
                                                   IReadOnlyCollection<Communication> communications,
                                                   NotificationChannel channels = NotificationChannel.Any,
                                                   CancellationToken cancellationToken = default)
        where TNotification : class
    {
        if (communications.Count == 0)
        {
            _logger.LogWarning("Unable to dispatch notification={@Notification}: Communications list is empty",
                notification);
            return;
        }

        var transports = ResolveTransports(channels);

        var lookup = transports.DistinctBy(x => x.GetType())
                               .Where(t => communications.Any(t.CanSend))
                               .Select(t => (
                                   Transport: t,
                                   RendererType: typeof(INotificationRenderer<,>).MakeGenericType(
                                       typeof(TNotification), t.GetType())))
                               .Select(t => (t.Transport, Renderer: _scope.TryResolve(t.RendererType, out var renderer)
                                   ? renderer
                                   : null))
                               .Where(x => x.Renderer != null)
                               .ToLookup(x => x.Transport, x => x.Renderer!);

        foreach (var channel in lookup)
        {
            var renderers = channel.ToList();

            if (renderers.Count == 0)
            {
                throw new ArgumentException(
                    $"No renderer registered for {typeof(TNotification).FullName} and {channel.Key.GetType().FullName}");
            }

            var renderedNotifications = new List<Task<Notification>>();
            var arguments = new[] { typeof(TNotification), channel.Key.GetType() };
            foreach (var renderer in renderers)
            {
                renderer.GetType()
                        .GetMethod(
                            nameof(INotificationRenderer<object, INotificationTransport>.RenderAsync),
                            arguments)!
                        .Invoke(renderer, new object[] { notification, channel.Key })!
                        .As<Task<Notification>>()
                        .Also(renderedNotifications.Add)
                        .Let(x => x.Id);
            }

            await Task.WhenAll(renderedNotifications);
            await Task.WhenAll(renderedNotifications.Select(
                x => communications.Where(channel.Key.CanSend)
                                   .ToList()
                                   .Let(c => c.Count > 0
                                       ? channel.Key.SendBatchAsync(x.Result, c, cancellationToken)
                                       : Task.CompletedTask)));
        }
    }

    private IEnumerable<INotificationTransport> ResolveTransports(NotificationChannel channels) =>
        Enum.GetValues<NotificationChannel>()
            .Let(q => channels.HasFlag(NotificationChannel.Any)
                ? q.Where(x => x != NotificationChannel.Any)
                : q.Where(x => channels.HasFlag(x)))
            .Select(x => _scope.ResolveKeyed<INotificationTransport>(x));
}