using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fab.Web.Swagger;

public class InternalServerErrorResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses.Add("500", new OpenApiResponse
        {
            Description = "Что-то пошло не так",
        });
    }
}