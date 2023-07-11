using Fab.Entities.Enums.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.CreateUserCommunication;

public class CreateUserCommunicationRequestValidator : AbstractValidator<CreateUserCommunicationRequest>
{
    public CreateUserCommunicationRequestValidator(IDbContext dbContext)
    {
        RuleFor(x => x.Value).NotEmpty()
                             .Communication(x => x.Type)
                             .WithMessage("Некорректный формат коммуникации");

        RuleFor(x => x.DeviceId).NotEmpty()
                                .WithMessage("DeviceId не заполнен")
                                .When(x => x.Type == CommunicationType.FirebasePushToken);

        RuleFor(x => x.DeviceId).MustAsync((request, _, cancellationToken) =>
                                    dbContext.Communications
                                             .Where(x => x.UserId == request.UserId &&
                                                         x.Type == request.Type &&
                                                         x.DeviceId == request.DeviceId)
                                             .AnyAsync(cancellationToken)
                                             .ContinueWith(x => !x.Result, cancellationToken))
                                .WithMessage("Коммуникация для этого устройства уже существует")
                                .When(x => x.DeviceId != null);
    }
}