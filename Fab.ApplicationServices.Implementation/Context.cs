using Fab.ApplicationServices.Interfaces;
using Fab.Entities.Enums.Users;

namespace Fab.ApplicationServices.Implementation;

public class Context : IContext
{
    public bool IsAuthenticated { get; set; }
    public Guid UserId { get; set; }
    public Role Role { get; set; } = Role.User;
}