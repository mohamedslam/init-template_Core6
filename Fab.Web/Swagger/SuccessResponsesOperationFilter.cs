using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Fab.Web.Swagger;

public class SuccessResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Responses["200"].Description == "Success")
        {
            operation.Responses["200"].Description = "Успех";
        }

        if (context.MethodInfo.ReturnType == typeof(Task))
        {
            ChangeOperationStatusCode(operation, "200", "204");
        }

        if (context.MethodInfo.GetCustomAttribute<HttpPostAttribute>() != null)
        {
            ChangeOperationStatusCode(operation, "200", "201");
        }
    }

    private static void ChangeOperationStatusCode(OpenApiOperation operation, string from, string to)
    {
        var response = operation.Responses.SingleOrDefault(x => x.Key == from).Value;
        if (response == null) return;

        if (operation.Responses.ContainsKey(to))
        {
            operation.Responses[to] = response;
        }
        else
        {
            operation.Responses.Add(to, response);
        }

        operation.Responses.Remove(from);
    }
}