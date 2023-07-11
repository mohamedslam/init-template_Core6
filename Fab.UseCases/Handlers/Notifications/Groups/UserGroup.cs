using Fab.Entities.Models.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Notifications.Groups;

public class UserGroup : INotificationGroup
{
    public IReadOnlyCollection<Guid> UserIds { get; }

    public UserGroup(params Guid[] userIds)
    {
        if (userIds is not { Length: > 0 })
        {
            throw new ArgumentNullException(nameof(userIds), "Expected at least one userId");
        }

        UserIds = userIds;
    }

    public async Task<IEnumerable<Communication>> ResolveRecipientsAsync(IReadonlyDbContext dbContext,
                                                                         CancellationToken cancellationToken) =>
        await dbContext.Users
                       .AsNoTracking()
                       .Where(x => UserIds.Contains(x.Id))
                       .SelectMany(x => x.Communications)
                       .ToListAsync(cancellationToken);
}