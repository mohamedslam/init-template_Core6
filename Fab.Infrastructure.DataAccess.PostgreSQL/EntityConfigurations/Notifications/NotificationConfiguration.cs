using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fab.Entities.Models.Notifications;

namespace Fab.Infrastructure.DataAccess.PostgreSQL.EntityConfigurations.Notifications;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasCheckConstraint("EntityIsComplex",
            $@"""{nameof(Notification.EntityType)}"" is null and ""{nameof(Notification.EntityId)}"" is null or " +
            $@"""{nameof(Notification.EntityType)}"" is not null and ""{nameof(Notification.EntityId)}"" is not null");
    }
}