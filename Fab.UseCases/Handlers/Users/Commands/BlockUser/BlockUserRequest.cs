using Fab.Entities.Abstractions;
using Fab.Entities.Models.Users;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Users.Commands.BlockUser;

public class BlockUserRequest : IRequest, IScopedRequest<User>
{
    [NotMapped]
    public Spec<User>? Scope { get; set; }

    [NotMapped]
    public Guid UserId { get; set; }
}