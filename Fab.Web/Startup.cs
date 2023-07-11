using AltPoint.Filters;
using AltPoint.Filters.Ast.Lexers;
using AltPoint.Filters.Ast.Parsers;
using Autofac;
using Fab.ApplicationServices.Implementation;
using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.DataAccess.PostgreSQL.Module;
using Fab.Infrastructure.Implementation;
using Fab.Infrastructure.Implementation.Sms.Zanzara;
using Fab.Infrastructure.Interfaces.Email;
using Fab.Infrastructure.Interfaces.Push;
using Fab.Infrastructure.Interfaces.Resources;
using Fab.Infrastructure.Interfaces.Sms;
using Fab.UseCases;
using Fab.Utils.Exceptions;
using Fab.Utils.Extensions;
using Fab.Web.Extensions;
using Fab.Web.HealthChecks;
using Fab.Web.Hubs.Notifications;
using Fab.Web.Middlewares;
using Fab.Web.Policies.Support;
using Fab.Web.Support;
using Fab.Web.Support.Json;
using Fab.Web.Swagger;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RazorLight;
using RazorLight.Extensions;
using Sentry;
using Sentry.AspNetCore;
using Sentry.Extensions.Logging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AuthenticationOptions = Fab.Infrastructure.Interfaces.Authentication.AuthenticationOptions;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Fab.Web;

public class Startup
{
    public static void ConfigureSentry(SentryAspNetCoreOptions options)
    {
        options.AddExceptionFilterForType<RestException>();
        options.AddExceptionFilterForType<ValidationException>();
        options.AddExceptionFilterForType<ApplicationException>();
        options.AddExceptionFilterForType<FilterTokenizeException>();
        options.AddExceptionFilterForType<FilterParseException>();

        options.AddLogEntryFilter((message, level, _, exception) => (level, exception) switch
        {
            (LogLevel.Warning, _) => !Regex.IsMatch(message, @"^Validator for [\w\.]* not found$"),
            _ => true
        });

        options.BeforeSend = e =>
        {
            foreach (var (key, _) in e.Request.Headers
                                      .Where(x => x.Key.StartsWith("X-"))
                                      .ToList())
            {
                e.Request.Headers
                 .Remove(key);
            }

            return e;
        };
    }

    private static void ConfigureJsonSerializer(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.Converters.Add(new JsonStringEnumConverter(options.PropertyNamingPolicy));
        options.Converters.Add(new NullableStringConverter());
        options.Converters.Add(new PolymorphicJsonConverter());
        options.Converters.Add(new DateOnlyConverter());
        options.Converters.Add(new TimeOnlyConverter());
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.PostConfigure<SentryAspNetCoreOptions>(ConfigureSentry);

        services.AddControllers(options =>
                {
                    options.ModelMetadataDetailsProviders
                           .Add(new DataMemberBindingMetadataProvider());

                    options.ModelMetadataDetailsProviders
                           .Add(new SuppressValidationByAttributeMetadataProvider(
                               typeof(NotMappedAttribute)));

                    options.ModelMetadataDetailsProviders
                           .Add(new SuppressValidationByAttributeMetadataProvider(
                               typeof(ReadOnlyAttribute),
                               x => x is ReadOnlyAttribute { IsReadOnly: true }));
                })
                .AddJsonOptions(options => ConfigureJsonSerializer(options.JsonSerializerOptions));

        services.AddSignalR(options => options.EnableDetailedErrors = true)
                .AddJsonProtocol(options => ConfigureJsonSerializer(options.PayloadSerializerOptions));

        services.AddApiExceptionHandler();

        services.AddOptions<JsonOptions>()
                .Configure(options => ConfigureJsonSerializer(options.SerializerOptions));

        services.AddOptions<JsonSerializerOptions>()
                .Configure(ConfigureJsonSerializer);

        services.AddApplicationDbContext();

        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddOptions<DataAccessOptions>()
                .BindConfiguration("Database");

        services.AddOptions<AuthenticationOptions>()
                .BindConfiguration("Application:Authentication");

        services.AddOptions<CommunicationOptions>()
                .BindConfiguration("Application:Communications");

        services.AddOptions<SmsRuOptions>()
                .BindConfiguration("Application:Sms:SmsRu");

        services.AddOptions<ZanzaraOptions>()
                .BindConfiguration("Application:Sms:Zanzara")
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddOptions<EmailOptions>()
                .BindConfiguration("Application:Email");

        services.AddOptions<S3Options>()
            .BindConfiguration("Application:S3")
                .ValidateDataAnnotations();

        services.AddOptions<PushOptions>()
                .BindConfiguration("Application:Firebase")
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddOptions<SwaggerOptions>()
                .BindConfiguration("Application:Swagger");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

        services.AddAuthorization();

        services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        }));

        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddSwagger();

        services.AddFilters(options =>
            options.SetNameFormatter(s => s.Uncapitalize())
                   .ConfigureDefaultParserOptions(opt => opt.SetQueryLikeProvider(EF.Functions.ILike)
                                                            .SetCaseSensitivity(false)));

        services.AddHttpLogging(options =>
            options.LoggingFields = HttpLoggingFields.RequestHeaders | HttpLoggingFields.RequestBody);

        services.AddRazorLight(() => typeof(InfrastructureModule)
            .Let(type => new RazorLightEngineBuilder()
                         .UseMemoryCachingProvider()
                         .UseEmbeddedResourcesProject(type.Assembly, $"{type.Namespace}.Notifications.Templates")
                         .SetOperatingAssembly(type.Assembly)
                         .Build()));

        services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck<IDbContext>>(
                    "Database", HealthStatus.Degraded, timeout: TimeSpan.FromMinutes(1));

        services.AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = false;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'F";
                    options.SubstituteApiVersionInUrl = true;
                });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder
            .RegisterType<Mediator>()
            .As<IMediator>()
            .InstancePerLifetimeScope();

        var scopedServices = new[]
        {
            typeof(IMiddleware),
            typeof(IFilterMetadata),
            typeof(IAuthorizationHandler)
        };

        var singletonServices = new[]
        {
            typeof(IHostedService),
            typeof(IAuthorizationPolicyProvider),
            typeof(IAuthorizationMiddlewareResultHandler)
        };

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(t => scopedServices.Any(x => x.IsAssignableFrom(t)))
               .AsSelf()
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(t => singletonServices.Any(x => x.IsAssignableFrom(t)))
               .AsImplementedInterfaces()
               .SingleInstance();

        builder.RegisterModule<ApplicationModule>();
        builder.RegisterModule<DataAccessModule>();
        builder.RegisterModule<UseCasesModule>();
        builder.RegisterModule<InfrastructureModule>();

        builder.RegisterGeneric(typeof(ScopeBehavior<,>))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(PolicyBehavior<,>))
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .AsClosedTypesOf(typeof(INotificationHandler<>))
               .AsImplementedInterfaces();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseFiltersHotStart(env.IsDevelopment());
        app.UseApiExceptionHandler();

        if (env.IsDevelopment())
        {
            app.UseSwaggerPage();
        }

        app.UseApiVersioning();
        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseMiddleware<ContextMiddleware>();
        app.UseAuthorization();
        app.UseHttpLogging();

        app.UseSentryTracing();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
            endpoints.MapHub<NotificationsHub>("/hubs/notifications");
            endpoints.MapControllers();
        });
    }
}