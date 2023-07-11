using System.Diagnostics.CodeAnalysis;

namespace Fab.Web.Swagger;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class SwaggerOptions
{
    public Dictionary<string, Uri> Definitions { get; set; } = new();
    public List<Uri> Stylesheets { get; set; } = new();
}