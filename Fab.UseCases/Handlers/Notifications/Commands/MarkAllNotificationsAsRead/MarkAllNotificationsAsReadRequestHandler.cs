using Fab.ApplicationServices.Interfaces;
using Fab.Infrastructure.DataAccess.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Notifications.Commands.MarkAllNotificationsAsRead;

public class MarkAllNotificationsAsReadRequestHandler : IRequestHandler<MarkAllNotificationsAsReadRequest>
{
    private readonly IContext _context;
    private readonly IDbContext _dbContext;

    public MarkAllNotificationsAsReadRequestHandler(IContext context, IDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(MarkAllNotificationsAsReadRequest request, CancellationToken cancellationToken)
    {
        var notifications = await _dbContext.Notifications
                                           .Where(x => x.ReceiverId == _context.UserId)
                                           .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        foreach (var notification in notifications)
        {
            notification.ReadAt ??= now;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}