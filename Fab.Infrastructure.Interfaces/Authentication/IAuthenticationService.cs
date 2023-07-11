using Fab.Entities.Models.Users;

namespace Fab.Infrastructure.Interfaces.Authentication;

public interface IAuthenticationService
{
    AuthToken GenerateToken(User user, ITokenInfo tokenInfo);
    
    Guid VerifyAccessToken(string token);
    
    Task<(AuthToken Auth, User User)> RefreshTokenAsync(string token,
                                                        CancellationToken cancellationToken = default);
}