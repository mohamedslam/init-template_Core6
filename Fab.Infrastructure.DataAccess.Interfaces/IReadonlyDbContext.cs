using Fab.Entities.Models.Communications;
using Fab.Entities.Models.Customers;
using Fab.Entities.Models.Notifications;
using Fab.Entities.Models.Projects;
using Fab.Entities.Models.Resources;
using Fab.Entities.Models.Tags;
using Fab.Entities.Models.Tenants;
using Fab.Entities.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Fab.Infrastructure.DataAccess.Interfaces;

public interface IReadonlyDbContext : IAsyncDisposable, IDisposable
{
    DbSet<User> Users { get; }
    DbSet<Communication> Communications { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Tag> Tags { get; }
    DbSet<Resource> Resources { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Tenant> Tenants { get; }
    DbSet<TenantUsers> TenantsUsers { get; }
    DbSet<Project> Projects { get; }

    #region DbContext Properties & Methods

    /// <inheritdoc cref="DbContext.Set{T}()"/>
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    /// <inheritdoc cref="DbContext.Set{T}(string)"/>
    DbSet<TEntity> Set<TEntity>(string name)
        where TEntity : class;

    /// <inheritdoc cref="DbContext.Entry{TEntity}(TEntity)"/>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        where TEntity : class;

    /// <inheritdoc cref="DbContext.Model"/>
    IModel Model { get; }

    /// <inheritdoc cref="DbContext.ContextId"/>
    DbContextId ContextId { get; }

    /// <inheritdoc cref="DbContext.Database"/>
    DatabaseFacade Database { get; }

    #endregion
}