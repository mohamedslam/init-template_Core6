using Fab.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fab.Infrastructure.DataAccess.PostgreSQL.Module;

public static class DataAccessService
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services)
    {
        var builder = CreateDbContextOptionsBuilder();

        services.AddDbContext<ApplicationDbContext>(builder);
        services.AddDbContextFactory<ApplicationDbContext>(builder);

        return services;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Configure(this DbContextOptionsBuilder builder,
                                 DataAccessOptions options, IServiceProvider? ctx) =>
        builder.UseLogging(options.Logging, ctx)
               .UseNpgsql(options.Connection,
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Namespace)
                         .UseNetTopologySuite());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DbContextOptionsBuilder UseLogging(this DbContextOptionsBuilder builder, bool enable,
                                                      IServiceProvider? ctx) =>
        builder.Let(b => enable
                   ? b.EnableDetailedErrors()
                      .EnableSensitiveDataLogging()
                   : b)
               .Let(b => enable && ctx != null
                   ? b.UseLoggerFactory(ctx.GetRequiredService<ILoggerFactory>())
                   : b);

    private static Action<IServiceProvider, DbContextOptionsBuilder> CreateDbContextOptionsBuilder() =>
        (ctx, options) => options.Configure(ctx.GetRequiredService<IOptions<DataAccessOptions>>()
                                               .Value, ctx);

    private static Action<DbContextOptionsBuilder> CreateDbContextOptionsBuilder(
        DataAccessOptions dataAccessOptions) =>
        options => options.Configure(dataAccessOptions, null);

    internal static DbContextOptions CreateDbContextOptions(DataAccessOptions dataAccessOptions) =>
        new DbContextOptionsBuilder()
            .Also(CreateDbContextOptionsBuilder(dataAccessOptions))
            .Options;

    internal static void RunMigrations(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
    {
        var migrations = context.Database
                                .GetPendingMigrations()
                                .ToList();

        if (migrations.Any())
        {
            var sw = new Stopwatch();

            sw.Start();
            context.Database.Migrate();
            sw.Stop();

            logger.LogInformation(string.Join(Environment.NewLine,
                    migrations.Select((_, i) => $"Migrate: {{Migration{i}}}")
                              .Prepend("Found {PendingCount} pending migrations")
                              .Append("Database migrated successfully in {ElapsedMilliseconds} ms")),
                Enumerable.Empty<object>()
                          .Concat(migrations)
                          .Prepend(migrations.Count)
                          .Append(sw.ElapsedMilliseconds)
                          .ToArray());
        }
        else
        {
            logger.LogInformation("Nothing to migrate");
        }
    }
}