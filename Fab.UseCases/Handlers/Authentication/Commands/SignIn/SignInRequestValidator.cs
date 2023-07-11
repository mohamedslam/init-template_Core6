using FluentValidation;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignIn;

public class SignInRequestValidator : AbstractValidator<SignInRequest>
{
    public SignInRequestValidator()
    {
       // RuleFor(x => x.Code).NotEmpty();
    }
}