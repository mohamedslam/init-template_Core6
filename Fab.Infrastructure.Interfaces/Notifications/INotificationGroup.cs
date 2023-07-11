using Fab.Entities.Models.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;

namespace Fab.Infrastructure.Interfaces.Notifications;

public interface INotificationGroup
{
    Task<IEnumerable<Communication>> ResolveRecipientsAsync(IReadonlyDbContext dbContext,
                                                            CancellationToken cancellationToken);
}