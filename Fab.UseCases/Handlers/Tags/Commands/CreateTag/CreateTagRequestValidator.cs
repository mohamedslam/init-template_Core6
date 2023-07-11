using FluentValidation;

namespace Fab.UseCases.Handlers.Tags.Commands.CreateTag;

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Label).NotEmpty()
                             .WithMessage("Заголовок тега не заполнен");
    }
}