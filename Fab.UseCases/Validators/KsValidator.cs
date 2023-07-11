using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class KsValidator<T> : PropertyValidator<T, string>
{
    private readonly Func<T, string> _bikResolver;

    public KsValidator(Func<T, string> bikResolver) =>
        _bikResolver = bikResolver;

    public override string Name => nameof(KsValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        value.Length == 20 &&
        value.All(char.IsDigit) &&
        _bikResolver(context.InstanceToValidate).Length == 9 &&
        CalculateChecksum($"0{_bikResolver(context.InstanceToValidate)[4..6]}{value}",
            new[] {7, 1, 3, 7, 1, 3, 7, 1, 3, 7, 1, 3, 7, 1, 3, 7, 1, 3, 7, 1, 3, 7, 1}) % 10 == 0;

    private static int CalculateChecksum(string value, IEnumerable<int> coefficients) =>
        value.ToCharArray()
             .Select(c => c - '0')
             .Zip(coefficients)
             .Aggregate(0, Calculate);

    private static int Calculate(int acc, (int Digit, int Coefficient) tuple) =>
        acc + tuple.Coefficient * (tuple.Digit % 10);

    protected override string GetDefaultMessageTemplate(string errorCode) =>
        "Неправильный формат корреспондентского счёта";
}