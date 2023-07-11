using Fab.Entities.Abstractions;
using Fab.Entities.Enums.Users;
using Fab.Entities.Models.Users;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Users.Commands.ChangeUserRole;

public class ChangeUserRoleRequest : IRequest, IScopedRequest<User>
{
    [NotMapped]
    public Spec<User>? Scope { get; set; }

    [NotMapped]
    public Guid UserId { get; set; }

    public Role Role { get; set; }
}