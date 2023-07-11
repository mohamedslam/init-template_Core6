using Fab.Entities.Models.Users;
using Fab.Infrastructure.Interfaces.Authentication;

namespace Fab.Infrastructure.Implementation.Authentication;

public static class TokenExtensions
{
    public static void Apply(this Token token, ITokenInfo info)
    {
        token.IpAddress = info.IpAddress;
        token.UserAgent = info.UserAgent;
    }

    public static bool IsMatch(this Token token, ITokenInfo info) =>
        Equals(token.IpAddress, info.IpAddress) && token.UserAgent == info.UserAgent;
}