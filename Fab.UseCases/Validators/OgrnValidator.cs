using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class OgrnValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(OgrnValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        value.Length == 13 &&
        value.All(char.IsDigit) &&
        ulong.TryParse(value[..^1], out var digit) &&
        byte.TryParse(value[^1..], out var checksum) &&
        digit % 11 % 10 == checksum;

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат ОГРН";
}