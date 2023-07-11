using Fab.Infrastructure.Interfaces.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fab.Infrastructure.Implementation.Authentication.Configurations;

public class TokenValidationConfiguration : IConfigureOptions<TokenValidationParameters>
{
    private readonly AuthenticationOptions _options;

    public TokenValidationConfiguration(IOptions<AuthenticationOptions> options) =>
        _options = options.Value;

    public void Configure(TokenValidationParameters options)
    {
        options.ValidateAudience = false;
        options.ValidateLifetime = true;
        options.ValidateIssuerSigningKey = true;
        options.ValidIssuer = _options.Issuer;
        options.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        options.ClockSkew = TimeSpan.Zero;
    }
}