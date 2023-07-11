using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Fab.Web.Support;

public class SuppressValidationByAttributeMetadataProvider : IValidationMetadataProvider
{
    private readonly Type _attribute;
    private readonly Func<object, bool> _condition = _ => true;

    public SuppressValidationByAttributeMetadataProvider(Type attribute) =>
        _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));

    public SuppressValidationByAttributeMetadataProvider(Type attribute, Func<object, bool> condition)
        : this(attribute) =>
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));

    public void CreateValidationMetadata(ValidationMetadataProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!(context.PropertyAttributes
                     ?.Any(x => x.GetType() == _attribute && _condition(x))
              ?? false))
        {
            return;
        }

        context.ValidationMetadata.IsRequired = false;

        foreach (var validator in context.ValidationMetadata
                                         .ValidatorMetadata
                                         .OfType<RequiredAttribute>()
                                         .ToList())
        {
            context.ValidationMetadata
                   .ValidatorMetadata
                   .Remove(validator);
        }
    }
}