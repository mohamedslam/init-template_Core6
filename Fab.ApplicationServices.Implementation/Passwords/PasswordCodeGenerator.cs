using Fab.ApplicationServices.Interfaces.Passwords;
using Fab.Entities.Enums.Communications;
using Fab.Entities.Enums.Users;
using Fab.Entities.Models.Users;

namespace Fab.ApplicationServices.Implementation.Passwords;

public class PasswordCodeGenerator : IPasswordCodeGenerator
{
    public string GenerateCode(CommunicationType type, User? user) =>
        (type, user?.Role ?? Role.User) switch
        {
            (CommunicationType.Email or CommunicationType.Phone, Role.User) => GenerateNumericCode(8),
            (CommunicationType.Email or CommunicationType.Phone, _) => GenerateNumericCode(8),

            _ => throw new ArgumentOutOfRangeException(
                nameof(type), type, "Неизвестный тип коммуникации")
        };

    private static string GenerateNumericCode(int length)
    {
        var from = (int)Math.Pow(10, length - 1);
        var to = from * 10;

        return Random.Shared
            .Next(from, to)
            .ToString();
    }

    private static string GenerateAlphaNumericCode(int length) =>
        Guid.NewGuid()
            .ToString()
            .Replace("-", string.Empty)
            [..length];
}