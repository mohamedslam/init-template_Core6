using Fab.UseCases.Validators;
using FluentValidation;

namespace Fab.UseCases.Handlers.Authentication.Commands.SendCode;

public class SendCodeRequestValidator : AbstractValidator<SendCodeRequest>
{
    public SendCodeRequestValidator()
    {
        RuleFor(x => x.Communication.Value).NotEmpty()
                                           .Communication(x => x.Communication.Type);
    }
}