using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Users;

namespace Fab.ApplicationServices.Interfaces.Passwords;

public interface IPasswordCodeGenerator
{
    string GenerateCode(CommunicationType type, User user);
}