using Fab.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Fab.Web.Swagger;

public class SwaggerConfiguration : IConfigureOptions<SwaggerGenOptions>, IConfigureOptions<SwaggerUIOptions>
{
    private const string ProjectName = "FAB API";
    private const string ProjectDescription = "Master FAB specification";

    private readonly IApiVersionDescriptionProvider _versionProvider;
    private readonly SwaggerOptions _options;

    public SwaggerConfiguration(IApiVersionDescriptionProvider versionProvider, IOptions<SwaggerOptions> options)
    {
        _versionProvider = versionProvider;
        _options = options.Value;
    }

    public void Configure(SwaggerGenOptions c)
    {
        foreach (var description in _versionProvider.ApiVersionDescriptions)
        {
            c.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = ProjectName,
                Description = description.IsDeprecated
                    ? "<code> /!\\  This API version has been deprecated  /!\\ </code>"
                    : ProjectDescription,
                Version = description.ApiVersion.ToString("F"),
            });
        }

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        });

        c.CustomOperationIds(api => api.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);

        c.UseOneOfForPolymorphism();
        c.UseAllOfForInheritance();
        c.UseInlineDefinitionsForEnums();
        c.UseAllOfToExtendReferenceSchemas();

        // Set the comments path for the Swagger JSON and UI.
        c.IncludeXmlComments();

        c.MapType<DateOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "date"
        });

        c.MapType<TimeOnly>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "time",
            Pattern = @"^\d{2}:\d{2}$",
            Example = new OpenApiString("14:52")
        });


        c.OperationFilter<AuthResponsesOperationFilter>();
        c.OperationFilter<SuccessResponsesOperationFilter>();
        c.OperationFilter<InternalServerErrorResponsesOperationFilter>();
        c.OperationFilter<ResponsesSchemaOperationFilter>();
        c.SchemaFilter<SkipPropertySchemaFilter>();
        c.SchemaFilter<NullableReferenceTypesSchemaFilter>();
        c.SchemaFilter<ModelNamingSchemaFilter>();
        c.OperationFilter<SkipPropertySchemaFilter>();
        c.DocumentFilter<RemoveVersionFromParameter>();
    }

    public void Configure(SwaggerUIOptions c)
    {
        c.DocumentTitle = ProjectName;

        foreach (var description in _versionProvider.ApiVersionDescriptions
                                                    .Reverse())
        {
            var name = $"{ProjectName} {description.ApiVersion.ToString("'v'FF")}";

            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.IsDeprecated
                ? $"[ Deprecated ] {name}"
                : name);
        }

        foreach (var (name, uri) in _options.Definitions)
        {
            c.SwaggerEndpoint(uri.ToString(), name);
        }

        c.DisplayOperationId();
        c.ShowExtensions();
        c.DefaultModelRendering(ModelRendering.Model);
        c.EnablePersistAuthorization();

        foreach (var stylesheet in _options.Stylesheets)
        {
            c.InjectStylesheet(stylesheet.ToString());
        }
    }
}