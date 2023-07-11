using Fab.Entities.Enums.Communications;
using FluentValidation;
using FluentValidation.Validators;

namespace Fab.UseCases.Validators;

public class CommunicationValidator<T> : PropertyValidator<T, string>
{
    private readonly Func<T, CommunicationType> _communicationTypeResolver;

    public CommunicationValidator(Func<T, CommunicationType> communicationTypeResolver) =>
        _communicationTypeResolver = communicationTypeResolver;

    public override string Name => nameof(CommunicationValidator<object>);

    public override bool IsValid(ValidationContext<T> context, string value) =>
        _communicationTypeResolver(context.InstanceToValidate) switch
        {
            CommunicationType.Email => new EmailValidator<T>().IsValid(context, value),
            CommunicationType.Phone => new PhoneValidator<T>().IsValid(context, value),
            CommunicationType.FirebasePushToken => !string.IsNullOrWhiteSpace(value),
            _ => throw new ArgumentOutOfRangeException(nameof(value),
                "Unexpected type of CommunicationType during validation")
        };

    protected override string GetDefaultMessageTemplate(string errorCode) => 
        "Неверный тип или значение коммуникации";
}