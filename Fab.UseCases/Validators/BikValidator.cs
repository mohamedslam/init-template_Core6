using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class BikValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(BikValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        value.Length == 9 &&
        value.All(char.IsDigit);

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат БИК";
}