using Fab.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Fab.Web.Swagger;

public class AuthResponsesOperationFilter : IOperationFilter
{
    private static readonly OpenApiSecurityRequirement JwtSecurity = new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    public IEnumerable<AuthorizeAttribute> GetSecurityAttributes(MethodInfo? method)
    {
        if (method == null ||
            method.GetCustomAttribute<AllowAnonymousAttribute>() != null)
        {
            return Enumerable.Empty<AuthorizeAttribute>();
        }

        return method.DeclaringType
                     ?.GetCustomAttributes(true)
                     .Union(method.GetCustomAttributes(true))
                     .OfType<AuthorizeAttribute>()
               ?? Enumerable.Empty<AuthorizeAttribute>();
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var security = GetSecurityAttributes(context.MethodInfo).ToList();

        if (security.Count == 0)
        {
            return;
        }

        operation.Security.Add(JwtSecurity);

        var roles = security.Select(x => x.Roles)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .SelectMany(x => x!.Split(",", StringSplitOptions.TrimEntries))
                            .ToList();

        var policies = security.Select(x => x.Policy)
                               .Where(x => !string.IsNullOrWhiteSpace(x))
                               .ToList();

        operation.Extensions["X-Roles"] = roles.Count == 0
            ? new OpenApiArray
            {
                new OpenApiString("*")
            }
            : new OpenApiArray()
                .Also(x => x.AddRange(roles.Select(role => new OpenApiString(role))));

        if (policies.Count > 0)
        {
            operation.Extensions["X-Policies"] = new OpenApiArray()
                .Also(x => x.AddRange(policies.Select(policy => new OpenApiString(policy))));
        }
    }
}