using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class TinValidator<T> : PropertyValidator<T, string>
{
    public override string Name => nameof(TinValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (string.IsNullOrEmpty(value) || value.Any(c => !char.IsDigit(c)))
        {
            return false;
        }

        return value.Length switch
        {
            10 => IsValidTenDigitForm(Parse(value)),
            12 => IsValidTwelveDigitForm(Parse(value)),
            _ => false
        };
    }

    private static List<int> Parse(string value) =>
        value.ToCharArray()
             .Select(c => c - '0')
             .ToList();

    private static bool IsValidTenDigitForm(IReadOnlyList<int> digits) =>
        digits[9] == CalculateChecksum(digits, new[] {2, 4, 10, 3, 5, 9, 4, 6, 8});

    private static bool IsValidTwelveDigitForm(IReadOnlyList<int> digits) =>
        digits[10] == CalculateChecksum(digits, new[] {7, 2, 4, 10, 3, 5, 9, 4, 6, 8}) &&
        digits[11] == CalculateChecksum(digits, new[] {3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8});

    private static int CalculateChecksum(IEnumerable<int> digits, IEnumerable<int> coefficients) =>
        digits.Zip(coefficients)
              .Aggregate(0, Calculate) % 11 % 10;

    private static int Calculate(int acc, (int Digit, int Coefficient) tuple) =>
        acc + tuple.Digit * tuple.Coefficient;

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат ИНН";
}