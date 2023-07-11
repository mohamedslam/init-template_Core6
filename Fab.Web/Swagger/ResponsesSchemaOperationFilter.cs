using Fab.Web.Exceptions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net.Mime;

namespace Fab.Web.Swagger;

public class ResponsesSchemaOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses
                                          .Where(x => int.TryParse(x.Key, out var code) &&
                                                      code == 500)
                                          .Select(x => x.Value)
                                          .Where(x => x.Content.Count == 0))
        {
            response.Content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    MediaTypeNames.Application.Json, new OpenApiMediaType
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(
                            typeof(ExceptionDto), context.SchemaRepository)
                    }
                }
            };
        }
    }
}