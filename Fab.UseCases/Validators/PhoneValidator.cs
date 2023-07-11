using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class PhoneValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(PhoneValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        RegularExpressions.Phone.IsMatch(value);

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат телефонного номера";
}