using Fab.Infrastructure.DataAccess.PostgreSQL.Module;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fab.Infrastructure.DataAccess.PostgreSQL;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private readonly DataAccessOptions _options;

    public DesignTimeDbContextFactory()
    {
        _options = new ConfigurationBuilder()
                   .SetBasePath(GetBasePath())
                   .AddJsonFile("appsettings.json")
                   .AddJsonFile("appsettings.Development.json", optional: true)
                   .Build()
                   .GetSection("Database")
                   .Get<DataAccessOptions>();
    }

    private static string GetBasePath() => 
        Path.Combine(Directory.GetCurrentDirectory(), "../Fab.Web");

    public ApplicationDbContext CreateDbContext(string[] args) => 
        new(DataAccessService.CreateDbContextOptions(_options),
            LoggerFactory.Create(options => options.AddConsole())
                         .CreateLogger<ApplicationDbContext>());
}