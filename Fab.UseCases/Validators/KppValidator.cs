using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class KppValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(KppValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        RegularExpressions.Kpp.IsMatch(value);

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат КПП";
}