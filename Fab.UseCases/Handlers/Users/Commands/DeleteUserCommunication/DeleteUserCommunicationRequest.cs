using MediatR;

namespace Fab.UseCases.Handlers.Users.Commands.DeleteUserCommunication;

public class DeleteUserCommunicationRequest : IRequest
{
    public Guid UserId { get; set; }
    public Guid CommunicationId { get; set; }
}