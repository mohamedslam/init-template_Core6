using System.ComponentModel.DataAnnotations.Schema;
using MediatR;

namespace Fab.UseCases.Handlers.Users.Commands.VerifyUserCommunication;

public class SendVerifyCodeRequest : IRequest
{
    [NotMapped]
    public Guid UserId { get; set; }

    [NotMapped]
    public Guid CommunicationId { get; set; }
}