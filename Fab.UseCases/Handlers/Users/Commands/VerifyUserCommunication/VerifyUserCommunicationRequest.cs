using System.ComponentModel.DataAnnotations.Schema;
using MediatR;

namespace Fab.UseCases.Handlers.Users.Commands.VerifyUserCommunication;

public class VerifyUserCommunicationRequest : IRequest
{
    [NotMapped]
    public Guid UserId { get; set; }

    [NotMapped]
    public Guid CommunicationId { get; set; }

    /// <summary>
    ///     Одноразовый код подтверждения
    /// </summary>
    public string Code { get; set; } = null!;
}