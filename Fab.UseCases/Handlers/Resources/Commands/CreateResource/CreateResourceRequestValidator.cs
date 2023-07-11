using FluentValidation;

namespace Fab.UseCases.Handlers.Resources.Commands.CreateResource;

public class CreateResourceRequestValidator : AbstractValidator<CreateResourceRequest>
{
    public CreateResourceRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
                            .WithMessage("Название ресурса не заполнено");

        RuleFor(x => x.File).NotNull()
                            .WithMessage("Файл не прикреплён");
    }
}