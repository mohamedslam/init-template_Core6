using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Templates;
using Serilog.Templates.Themes;

namespace Fab.Web;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .WriteTo.Console(theme: AnsiConsoleTheme.Literate, applyThemeToRedirectedOutput: true)
                     .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web host");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((ctx, services, conf) =>
                conf.ReadFrom.Configuration(ctx.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(new ExpressionTemplate(
                        "[{@t:HH:mm:ss} {@l:u3}] [{SourceContext}{#if RequestId is not null} | {RequestId}{#end}] {@m}\n{@x}",
                        theme: TemplateTheme.Literate,
                        applyThemeWhenOutputIsRedirected: true)))
            .ConfigureWebHostDefaults(webBuilder =>
                webBuilder.UseStartup<Startup>()
                          .UseSentry());
}