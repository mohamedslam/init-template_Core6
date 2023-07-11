using Fab.Entities.Abstractions;
using Fab.Entities.Models.Communications;
using Fab.UseCases.Dto;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Users.Queries.ReadUserCommunication;

public class ReadUserCommunicationRequest : IRequest<CommunicationDto>, IScopedRequest<Communication>
{
    [NotMapped]
    public Spec<Communication>? Scope { get; set; }

    [NotMapped]
    public Guid UserId { get; set; }

    [NotMapped]
    public Guid CommunicationId { get; set; }
}