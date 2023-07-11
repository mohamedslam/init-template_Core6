using Fab.Web.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Fab.Web.Extensions;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen()
                .ConfigureOptions<SwaggerConfiguration>();

    public static IApplicationBuilder UseSwaggerPage(this IApplicationBuilder app) =>
        app.UseSwagger()
           .UseSwaggerUI();

    private static readonly Lazy<IReadOnlyCollection<XDocument>> XmlDocumentation = new(() =>
    {
        var xmlDocPaths = Directory.GetFiles(
            path: AppContext.BaseDirectory,
            searchPattern: $"{typeof(Startup).Namespace!.Split('.', 2).First()}.*.xml"
        );

        // load the XML docs for processing.
        var xmlDocs = xmlDocPaths.Select(XDocument.Load).ToList();

        // need a map for looking up member elements by name.
        var targetMemberElements = xmlDocs
                                   .Select(doc => doc
                                       .XPathSelectElements("/doc/members/member[@name and not(inheritdoc)]"))
                                   .SelectMany(members => members)
                                   .ToDictionary(m => m.Attribute("name")!.Value);

        // add member elements across all XML docs to the look-up table. We want <member> elements
        // that have a 'name' attribute but don't contain an <inheritdoc> child element.

        // for each <member> element that has an <inheritdoc> child element which references another
        // <member> element, replace the <inheritdoc> element with the nodes of the referenced <member>
        // element (effectively this 'dereferences the pointer' which is something Swagger doesn't support).
        foreach (var doc in xmlDocs)
        {
            var pointerMembers = doc.XPathSelectElements("/doc/members/member[inheritdoc[@cref]]");

            foreach (var pointerMember in pointerMembers)
            {
                var pointerElement = pointerMember.Element("inheritdoc");
                var targetMemberName = pointerElement!.Attribute("cref")!.Value;

                if (targetMemberElements.TryGetValue(targetMemberName, out var targetMember))
                    pointerElement.ReplaceWith(targetMember.Nodes());
            }
        }

        // replace all <see> elements with the unqualified member name that they point to (Swagger uses the
        // fully qualified name which makes no sense because the relevant classes and namespaces are not useful
        // when calling an API over HTTP).
        foreach (var doc in xmlDocs)
        {
            foreach (var seeElement in doc.XPathSelectElements("//see[@cref]"))
            {
                var targetMemberName = seeElement.Attribute("cref")!.Value;
                var shortMemberName = targetMemberName[(targetMemberName.LastIndexOf('.') + 1)..];

                if (targetMemberName.StartsWith("M:")) shortMemberName += "()";

                seeElement.ReplaceWith(shortMemberName);
            }
        }

        return xmlDocs;
    });

    /// <summary>
    ///     For more information see
    ///     <a href="https://github.com/domaindrivendev/Swashbuckle.WebApi/issues/1000#issuecomment-821813257">
    ///         GitHub issue</a>
    /// </summary>
    public static void IncludeXmlComments(this SwaggerGenOptions options)
    {
        // add pre-processed XML docs to Swagger.
        foreach (var doc in XmlDocumentation.Value)
            options.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()), true);
    }
}