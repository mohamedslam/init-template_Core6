using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Authentication.Commands.SignUp;
using FluentValidation;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignUpConfirm;

public class SignUpConfirmRequestValidator: AbstractValidator<SignUpConfirmRequest>
{
    public  SignUpConfirmRequestValidator(IDbContext dbContext)
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Должны вставить подтверждение кода");
    }
}
