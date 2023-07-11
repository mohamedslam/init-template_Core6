using Fab.Entities.Enums.Communications;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Users.Commands.CreateUserCommunication;

public class CreateUserCommunicationRequest : IRequest<Guid>
{
    [NotMapped]
    public Guid UserId { get; set; }

    /// <inheritdoc cref="CommunicationType"/>
    public CommunicationType Type { get; set; }

    /// <summary>
    ///     Значение коммуникации
    /// </summary>
    public string Value { get; set; } = null!;

    /// <summary>
    ///     Идентификатор устройства
    ///
    ///     Гарантируется что в рамках одного типа для одного устройства может существовать только одна коммуникация
    /// </summary>
    public string? DeviceId { get; set; } = null;
}