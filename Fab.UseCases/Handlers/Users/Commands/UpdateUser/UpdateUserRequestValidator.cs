using Fab.Infrastructure.DataAccess.Interfaces;
using FluentValidation;

namespace Fab.UseCases.Handlers.Users.Commands.UpdateUser;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IDbContext dbContext)
    {
        //Validation does not work for the required string property
        RuleFor(x => x.Name).NotEmpty()
                            .WithMessage("Имя должно быть заполнено");
    }
}