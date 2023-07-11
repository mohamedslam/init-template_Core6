using Fab.Entities.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Fab.Infrastructure.DataAccess.PostgreSQL.Extensions;

public static class DbContextExtensions
{
    #region Timestamps

    public static DbContext UseTimestamps(this DbContext context)
    {
        context.SavingChanges += OnSavingChangesUpdateTimestamps;
        return context;
    }

    private static void OnSavingChangesUpdateTimestamps(object? sender,
                                                        SavingChangesEventArgs savingChangesEventArgs)
    {
        if (sender is not DbContext context) return;
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted &&
                entry.Entity is ISoftDeletes { DeletedAt: null } softDeletesEntity)
            {
                entry.State = EntityState.Modified;
                softDeletesEntity.DeletedAt = now;

                foreach (var owned in entry.References
                                           .Select(x => x.TargetEntry))
                {
                    if (owned?.Metadata.FindOwnership() != null)
                    {
                        owned.State = EntityState.Unchanged;
                    } 
                }
            }

            if (entry.Entity is IHasTimestamps entityWithTimestamps)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entityWithTimestamps.CreatedAt == DateTime.MinValue)
                        {
                            entityWithTimestamps.CreatedAt = now;
                        }

                        entityWithTimestamps.UpdatedAt = now;
                        break;

                    case EntityState.Modified:
                        var createdAtProperty = entry.Property(nameof(IHasTimestamps.CreatedAt));

                        if (createdAtProperty.IsModified &&
                            (DateTime) createdAtProperty.CurrentValue! != (DateTime) createdAtProperty.OriginalValue!)
                        {
                            context.GetService<ILogger<DbContext>>()
                                   .LogWarning(
                                       "Attempt to update CreatedAt timestamp for entity \"{EntityType}\"\n" +
                                       "Entity ID: {EntityId}\n" +
                                       "Original Timestamp: {OriginalTimestamp}\n" +
                                       "Current Timestamp:  {CurrentTimestamp}\n" +
                                       "Changes will be reverted",
                                       entry.Entity.GetType().FullName,
                                       entry.Property(nameof(IEntity.Id)).CurrentValue,
                                       createdAtProperty.OriginalValue,
                                       entityWithTimestamps.CreatedAt);

                            entityWithTimestamps.CreatedAt = (DateTime)createdAtProperty.OriginalValue;
                        }

                        entityWithTimestamps.UpdatedAt = now;
                        break;
                }
            }
        }
    }

    #endregion
}