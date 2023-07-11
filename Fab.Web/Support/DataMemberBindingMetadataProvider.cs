using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Runtime.Serialization;

namespace Fab.Web.Support;

public class DataMemberBindingMetadataProvider : IBindingMetadataProvider
{
    public void CreateBindingMetadata(BindingMetadataProviderContext context)
    {
        var dataMember = context.Attributes
                                .OfType<DataMemberAttribute>()
                                .FirstOrDefault();

        if (dataMember is { Name: not null })
        {
            context.BindingMetadata.BinderModelName = dataMember.Name;
        }
    }
}