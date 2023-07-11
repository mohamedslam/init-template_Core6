using Fab.Infrastructure.DataAccess.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace Fab.Web.HealthChecks;

public class DatabaseHealthCheck<TContext> : IHealthCheck
    where TContext : IReadonlyDbContext
{
    private readonly TContext _context;
    private readonly TimeSpan _degradedThreshold;
    private readonly Stopwatch _sw = new();

    public DatabaseHealthCheck(TContext context) : this(context, TimeSpan.FromMilliseconds(1_000))
    {
    }

    public DatabaseHealthCheck(TContext context, TimeSpan degradedThreshold)
    {
        _context = context;
        _degradedThreshold = degradedThreshold;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                          CancellationToken cancellationToken = new())
    {
        try
        {
            _sw.Start();
            var result = await _context.Database
                                       .CanConnectAsync(cancellationToken);
            _sw.Stop();

            if (!result)
            {
                return HealthCheckResult.Unhealthy("Unable connect to database");
            }

            return _sw.ElapsedMilliseconds >= _degradedThreshold.TotalMilliseconds
                ? HealthCheckResult.Degraded($"Connection took {_sw.ElapsedMilliseconds}ms")
                : HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("Unable connect to database", e);
        }
        finally
        {
            _sw.Reset();
        }
    }
}