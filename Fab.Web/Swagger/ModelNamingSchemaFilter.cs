using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fab.Web.Swagger;

public class ModelNamingSchemaFilter : ISchemaFilter
{
    private readonly ILogger<ModelNamingSchemaFilter> _logger;

    public ModelNamingSchemaFilter(ILogger<ModelNamingSchemaFilter> logger) =>
        _logger = logger;

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (IsDomainModel(context))
        {
            _logger.LogWarning(@"Direct using domain model ""{Type}"" is not allowed", context.Type.FullName);
        }

        // например: Page<Item> -> ItemPage
        if (context.Type.IsGenericType &&
            !context.Type.IsInterface &&
            context.Type.GenericTypeArguments.Length == 1 &&
            context.Type.GenericTypeArguments.First().Name.Contains("Dto"))
        {
            schema.Title ??=
                $"{context.Type.GenericTypeArguments.First().Name.Replace("Dto", "")}{context.Type.Name[..^2]}";
        }

        if (context.Type.Name.Contains("Dto"))
        {
            schema.Title ??= context.Type.Name.Replace("Dto", "");
        }

        if (schema?.Properties != null)
        {
            foreach (var property in schema.Properties.Values)
            {
                if (property.Type == "array")
                {
                    property.Nullable = false;
                }
            }
        }
    }

    private static bool IsDomainModel(SchemaFilterContext context) =>
        context.Type.Namespace != null &&
        context.Type.Namespace.StartsWith("Fab.Entities.Models");
}