using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Users;

namespace Fab.ApplicationServices.Interfaces.Communications;

public interface IVerificationCodeGenerator
{
    string GenerateCode(CommunicationType type, User user);
}