using Fab.Entities.Enums.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Validators;
using FluentValidation;
using FluentValidation.Results;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignUp;

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    public SignUpRequestValidator(IDbContext dbContext)
    {
        RuleFor(x => x.Password).NotEmpty()
                               .WithMessage("Пароль должен быть заполнен");

        RuleForEach(x => x.Communications).ChildRules(v =>
            v.RuleFor(x => x.Type)
             .Must(x => x is CommunicationType.Email or CommunicationType.Phone)
             .WithMessage("Недопустимый тип коммуникации"));

        RuleFor(x => x.Communications).NotEmpty()
                                      .WithMessage("Коммуникации не переданы")
                                      .UniqueCommunications(dbContext.Communications.Where(x => x.UserId.HasValue), (propertyName) =>
                                          new ValidationFailure(propertyName, "Пользователь уже существует, войдите в систему"));

        RuleForEach(x => x.Communications).ChildRules(v =>
        {
            v.RuleFor(x => x.Value)
             .Communication(x => x.Type);
        });
    }
}