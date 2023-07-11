using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Fab.Web.Swagger;

public class NullableReferenceTypesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var (key, value) in schema.Properties)
        {
            var field = context.Type
                               .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                               .FirstOrDefault(x =>
                                   string.Equals(x.Name, key, StringComparison.InvariantCultureIgnoreCase));

            if (field != null)
            {
                var fieldType = field switch
                {
                    FieldInfo fieldInfo => fieldInfo.FieldType,
                    PropertyInfo propertyInfo => propertyInfo.PropertyType,
                    _ => throw new NotSupportedException("Unable to get member type"),
                };

                value.Nullable = fieldType.IsValueType
                    ? Nullable.GetUnderlyingType(fieldType) != null
                    : !field.IsNonNullableReferenceType();
            }
        }
    }
}