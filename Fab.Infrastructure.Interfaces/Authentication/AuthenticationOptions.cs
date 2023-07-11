namespace Fab.Infrastructure.Interfaces.Authentication;

public class AuthenticationOptions
{
    public string Issuer { get; set; } = null!;
    public string SigningKey { get; set; } = null!;
    public TimeSpan AccessTokenLifetime { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }

    public bool TryToRenewToken { get; set; } = true;
}