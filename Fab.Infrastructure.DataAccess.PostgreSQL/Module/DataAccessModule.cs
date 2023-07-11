using Autofac;
using Autofac.Features.OwnedInstances;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fab.Infrastructure.DataAccess.PostgreSQL.Module;

public sealed class DataAccessModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ApplicationDbContext>()
               .AsSelf()
               .As<IDbContext>()
               .InstancePerLifetimeScope();

        builder.Register(ctx =>
                   // пока у нас нет slave реплики бд, используем то же подключение
                   // если не сделать его Owned, то он может попортить не readonly контекст
                   ctx.Resolve<Owned<ApplicationDbContext>>().Value
                      .Also(x => x.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking))
               .As<IReadonlyDbContext>()
               .InstancePerLifetimeScope();

        builder.RegisterBuildCallback(Migrate);
    }

    private static void Migrate(ILifetimeScope scope)
    {
        var context = scope.Resolve<ApplicationDbContext>();
        var logger = scope.Resolve<ILogger<ApplicationDbContext>>();

        DataAccessService.RunMigrations(context, logger);
    }
}