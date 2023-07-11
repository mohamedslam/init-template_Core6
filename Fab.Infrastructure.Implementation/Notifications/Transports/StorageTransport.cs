using Autofac.Features.OwnedInstances;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Implementation.Notifications.Common.Transports;
using Fab.Infrastructure.Interfaces.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;
using SystemNotification = Fab.Entities.Models.Notifications.Notification;

namespace Fab.Infrastructure.Implementation.Notifications.Transports;

public class StorageTransport : BatchTransport
{
    private readonly IDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<StorageTransport> _logger;

    public StorageTransport(Owned<IDbContext> dbContext, IMediator mediator, ILogger<StorageTransport> logger)
    {
        _dbContext = dbContext.Value;
        _mediator = mediator;
        _logger = logger;
    }

    public override bool CanSend(Communication communication) =>
        communication.UserId.HasValue;

    public override async Task SendBatchAsync(Notification notification,
                                              IReadOnlyCollection<Communication> communications,
                                              CancellationToken cancellationToken = default)
    {
        var withoutUser = communications.Where(x => !x.UserId.HasValue)
                                        .Select(x => x.Id)
                                        .ToArray();

        if (withoutUser.Length > 0)
        {
            _logger.LogWarning("Some communications doesn't belongs to user: communications={@Communications}",
                withoutUser);
        }

        var notifications = communications.Where(x => x.UserId.HasValue)
                                          .DistinctBy(x => x.UserId)
                                          .Select(x => new SystemNotification
                                          {
                                              ReceiverId = x.UserId!.Value,

                                              EntityType = notification.GetExtra<string>(
                                                  nameof(SystemNotification.EntityType)),
                                              EntityId = notification.GetExtra<string>(
                                                  nameof(SystemNotification.EntityId)),

                                              Title = notification.Subject,
                                              Description = notification.Content
                                          })
                                          .ToList();

        if (notifications.Count == 0)
        {
            _logger.LogWarning("Unable to store notification={@Notification}", notification);
            return;
        }

        _dbContext.AddRange(notifications);
        _dbContext.OnCommit(() => Task.WhenAll(notifications.Select(x =>
            _mediator.Publish(new NotificationDispatched(x), CancellationToken.None))));

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}