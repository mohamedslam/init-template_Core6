using FluentValidation;

namespace Fab.UseCases.Handlers.Resources.Commands.UpdateResource;

public class UpdateResourceRequestValidator : AbstractValidator<UpdateResourceRequest>
{
    public UpdateResourceRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
                            .WithMessage("Название ресурса не заполнено");
    }
}