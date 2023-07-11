using FluentValidation;
using FluentValidation.Validators;
using System.Net.Mail;

namespace Fab.UseCases.Validators;

public class EmailValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "EmailValidator";

    public override bool IsValid(ValidationContext<T> context, string value) =>
        MailAddress.TryCreate(value, out _);
}