using Fab.Entities.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Fab.Infrastructure.DataAccess.Interfaces;

public interface IDbContext : IReadonlyDbContext
{
    /// <summary>
    ///     Восстановит мягко-удалённую сущность
    /// </summary>
    EntityEntry<TEntity> Restore<TEntity>(TEntity entity)
        where TEntity : class, ISoftDeletes;

    /// <summary>
    ///     Удалит сущность минуя мягкое удаление
    /// </summary>
    EntityEntry<TEntity> ForceRemove<TEntity>(TEntity entity)
        where TEntity : class, ISoftDeletes;

    void OnCommit(Action callback);
    void OnCommit(Func<Task> callback);
    void OnRollback(Action<Exception> callback);

    #region DbContext Properties & Methods

    /// <inheritdoc cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker"/>
    ChangeTracker ChangeTracker { get; }

    /// <inheritdoc cref="DbContext.Attach"/>
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        where TEntity : class;

    /// <inheritdoc cref="DbContext.AttachRange(object[])"/>
    void AttachRange(params object[] entities);

    /// <inheritdoc cref="DbContext.AttachRange(IEnumerable{object})"/>
    void AttachRange(IEnumerable<object> entities);

    /// <inheritdoc cref="DbContext.Add"/>
    EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        where TEntity : class;

    /// <inheritdoc cref="DbContext.AddRange(object[])"/>
    void AddRange(params object[] entities);

    /// <inheritdoc cref="DbContext.AddRange(IEnumerable{object})"/>
    void AddRange(IEnumerable<object> entities);

    /// <inheritdoc cref="DbContext.Update"/>
    EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        where TEntity : class;

    /// <inheritdoc cref="DbContext.UpdateRange(object[])"/>
    void UpdateRange(params object[] entities);

    /// <inheritdoc cref="DbContext.UpdateRange(IEnumerable{object})"/>
    void UpdateRange(IEnumerable<object> entities);

    /// <inheritdoc cref="DbContext.Remove"/>
    EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        where TEntity : class;

    /// <inheritdoc cref="DbContext.RemoveRange(object[])"/>
    void RemoveRange(params object[] entities);

    /// <inheritdoc cref="DbContext.RemoveRange(IEnumerable{object})"/>
    void RemoveRange(IEnumerable<object> entities);

    /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)"/>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    #endregion
}