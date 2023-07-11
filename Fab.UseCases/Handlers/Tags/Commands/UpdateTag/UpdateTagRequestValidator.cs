using FluentValidation;

namespace Fab.UseCases.Handlers.Tags.Commands.UpdateTag;

public class UpdateTagRequestValidator : AbstractValidator<UpdateTagRequest>
{
    public UpdateTagRequestValidator()
    {
        RuleFor(x => x.Label).NotEmpty()
                             .WithMessage("Заголовок тега не заполнен");
    }
}