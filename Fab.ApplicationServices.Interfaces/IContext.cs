using Fab.Entities.Enums.Users;

namespace Fab.ApplicationServices.Interfaces;

public interface IContext
{
    public bool IsAuthenticated { get; }
    public Guid UserId { get; }
    public Role Role { get; }
}