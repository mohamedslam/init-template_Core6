using Fab.Entities.Enums.Communications;
using FluentValidation;

namespace Fab.UseCases.Handlers.Authentication.Commands.ResetPasswordRequest
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Communication).Must(x => x.Type is CommunicationType.Email or CommunicationType.Phone)
                .WithMessage("Недопустимый тип коммуникации");
        }
    }
}