using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Models.Communications;
using Fab.Entities.Models.Customers;
using Fab.Entities.Models.Notifications;
using Fab.Entities.Models.Projects;
using Fab.Entities.Models.Resources;
using Fab.Entities.Models.Tags;
using Fab.Entities.Models.Tenants;
using Fab.Entities.Models.Users;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.DataAccess.PostgreSQL.Extensions;
using Fab.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#pragma warning disable CS8618

namespace Fab.Infrastructure.DataAccess.PostgreSQL;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ApplicationDbContext : DbContext, IDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Communication> Communications { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Tenant> Tenants { get; set;}
    public DbSet<TenantUsers> TenantsUsers { get; set;}
    public DbSet<Project> Projects { get; set;}
    
    private readonly ILogger<ApplicationDbContext> _logger;
    private readonly List<Task> _onCommitTasks = new();

    public ApplicationDbContext(DbContextOptions options, ILogger<ApplicationDbContext> logger) : base(options)
    {
        _logger = logger;
        this.UseTimestamps();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
        configurationBuilder.Properties<Enum>(x => x.HaveConversion<string>());

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
                    .HasPostgresExtension("postgis");

    public EntityEntry<TEntity> Restore<TEntity>(TEntity entity)
        where TEntity : class, ISoftDeletes
    {
        entity.DeletedAt = null;
        return Update(entity);
    }

    public EntityEntry<TEntity> ForceRemove<TEntity>(TEntity entity)
        where TEntity : class, ISoftDeletes
    {
        entity.DeletedAt = DateTime.UtcNow;
        return Remove(entity);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var result = base.SaveChanges(acceptAllChangesOnSuccess);

        if (_onCommitTasks.Count > 0)
        {
            var tasks = _onCommitTasks.ToArray();
            _onCommitTasks.Clear();

            Task.WaitAll(tasks);
        }

        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
                                                     CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        if (_onCommitTasks.Count > 0)
        {
            var tasks = _onCommitTasks.ToArray();
            _onCommitTasks.Clear();

            await Task.WhenAll(tasks);
        }

        return result;
    }

    public void OnCommit(Action callback)
    {
        void OnSavedChanges(object? _, SavedChangesEventArgs args)
        {
            SavedChanges -= OnSavedChanges;
            callback();
        }

        SavedChanges += OnSavedChanges;
    }

    public void OnCommit(Func<Task> callback)
    {
        void OnSavedChanges(object? sender, SavedChangesEventArgs args) =>
            Task.Run(async () =>
                {
                    try
                    {
                        SavedChanges -= OnSavedChanges;
                        await callback();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "An error occurred during execution of the OnCommit method");
                    }
                })
                .Also(_onCommitTasks.Add);

        SavedChanges += OnSavedChanges;
    }

    public void OnRollback(Action<Exception> callback)
    {
        void OnSaveChangesFailed(object? _, SaveChangesFailedEventArgs args)
        {
            SaveChangesFailed -= OnSaveChangesFailed;
            callback(args.Exception);
        }

        SaveChangesFailed += OnSaveChangesFailed;
    }
}