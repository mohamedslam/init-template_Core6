using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fab.Web.Swagger
{
    public class RemoveVersionFromParameter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var (path, item) in swaggerDoc.Paths
                                                   .ToList())
            {
                if (!path.Contains("v{version}"))
                {
                    continue;
                }

                swaggerDoc.Paths.Remove(path);
                swaggerDoc.Paths.Add(path.Replace("v{version}", $"v{swaggerDoc.Info.Version}"), item);

                foreach (var (_, operation) in item.Operations)
                {
                    operation.Parameters.Remove(
                        operation.Parameters
                                 .FirstOrDefault(x => x.Name == "version"));
                }
            }
        }
    }
}