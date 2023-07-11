using FluentValidation;

namespace Fab.UseCases.Handlers.Authentication.Commands.RefreshToken;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}