using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class OgrnipValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(OgrnipValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        value.Length == 15 &&
        value.All(char.IsDigit) &&
        ulong.TryParse(value[..^1], out var digit) &&
        byte.TryParse(value[^1..], out var checksum) &&
        digit % 13 % 10 == checksum;

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат ОГРНИП";
}