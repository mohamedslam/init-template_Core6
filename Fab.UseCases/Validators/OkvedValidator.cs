using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class OkvedValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(OkvedValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        value.Length == 6 && value.ToCharArray()
                                  .All(char.IsDigit);

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат ОКВЭД";
}