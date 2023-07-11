using Fab.Entities.Models.Communications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fab.Infrastructure.DataAccess.PostgreSQL.EntityConfigurations.Communications;

public class CommunicationsConfiguration : IEntityTypeConfiguration<Communication>
{
    public void Configure(EntityTypeBuilder<Communication> builder)
    {
        builder.HasIndex(x => new { x.Type, x.Value })
               .HasFilter($@"""{nameof(Communication.Confirmed)}"" = true");

        builder.HasIndex(x => new { x.UserId, x.Type, x.DeviceId })
               .HasFilter(
                   $@"""{nameof(Communication.UserId)}"" is not null and ""{nameof(Communication.DeviceId)}"" is not null")
               .IsUnique();

        builder.HasCheckConstraint("DeviceIdOnlyWithUser",
            $@"""{nameof(Communication.UserId)}"" is not null or ""{nameof(Communication.DeviceId)}"" is null");

        builder.HasCheckConstraint("ConfirmedHasUserId",
            $@"not ""{nameof(Communication.Confirmed)}"" or ""{nameof(Communication.UserId)}"" is not null");
    }
}