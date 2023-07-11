using Fab.ApplicationServices.Interfaces;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Notifications.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadRequestHandler : IRequestHandler<MarkNotificationAsReadRequest>
{
    private readonly IContext _context;
    private readonly IDbContext _dbContext;

    public MarkNotificationAsReadRequestHandler(IContext context, IDbContext dbContext)
    {
        _context = context;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(MarkNotificationAsReadRequest request, CancellationToken cancellationToken)
    {
        var notification = await _dbContext.Notifications
                                           .Where(x => x.ReceiverId == _context.UserId &&
                                                       x.Id == request.NotificationId)
                                           .FirstOrDefaultAsync(cancellationToken)
                           ?? throw new NotFoundException("Уведомление не найдено");

        notification.ReadAt ??= DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}