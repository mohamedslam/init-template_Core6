using Fab.Entities.Models.Users;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.Utils.Exceptions;
using Fab.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;

namespace Fab.Infrastructure.Implementation.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly JwtSecurityTokenHandler _jwtHandler;
    private readonly DateTime _accessTokenExpiresAt;
    private readonly DateTime _refreshTokenExpiresAt;
    private readonly IDbContext _context;
    private readonly TokenValidationParameters _parameters;
    private readonly AuthenticationOptions _options;

    public AuthenticationService(JwtSecurityTokenHandler jwtHandler, IDbContext context,
                                 IOptions<AuthenticationOptions> options,
                                 IOptions<TokenValidationParameters> parameters)
    {
        _context = context;
        _jwtHandler = jwtHandler;
        _options = options.Value;
        _parameters = parameters.Value;

        var now = DateTime.UtcNow;
        _accessTokenExpiresAt = now.Add(_options.AccessTokenLifetime);
        _refreshTokenExpiresAt = now.Add(_options.RefreshTokenLifetime);
    }

    #region GenerateToken

    public AuthToken GenerateToken(User user, ITokenInfo tokenInfo)
    {
        var token = user.Tokens
                        .FirstOrDefault(t => t.IsMatch(tokenInfo));

        if (token != null && _options.TryToRenewToken)
        {
            token.Renew();
        }
        else
        {
            token = new Token();
            token.Apply(tokenInfo);
            user.Tokens.Add(token);
        }

        return GenerateToken(user, token);
    }

    private AuthToken GenerateToken(User user, Token token)
    {
        if (user.Tokens.All(x => x.Id != token.Id))
        {
            user.Tokens.Add(token);
        }

        CleanupExpiredTokens(user);

        return new AuthToken
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = GenerateRefreshToken(token),
            ExpiresAt = _accessTokenExpiresAt
        };
    }

    private static IEnumerable<Claim> ResolveUserClaims(User user)
    {
        yield return new Claim("id", user.Id.ToString(), ClaimValueTypes.String);
        yield return new Claim(ClaimTypes.Role, user.Role.ToString(), ClaimValueTypes.String);
    }

    private static IEnumerable<Claim> ResolveTokenClaims(Token token)
    {
        yield return new Claim(JwtRegisteredClaimNames.Jti, token.Value.ToString(),
            ClaimValueTypes.String);
        yield return new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUnixTimestamp().ToString(),
            ClaimValueTypes.Integer64);
    }

    private string GenerateAccessToken(User user)
    {
        var jwt = new JwtSecurityToken(
            claims: ResolveUserClaims(user),
            issuer: _options.Issuer,
            expires: _accessTokenExpiresAt,
            signingCredentials: GetSigningCredentials()
        );

        return _jwtHandler.WriteToken(jwt);
    }

    private string GenerateRefreshToken(Token token)
    {
        var jwt = new JwtSecurityToken(
            claims: ResolveTokenClaims(token),
            issuer: _options.Issuer,
            notBefore: _accessTokenExpiresAt,
            expires: _refreshTokenExpiresAt,
            signingCredentials: GetSigningCredentials()
        );

        return _jwtHandler.WriteToken(jwt);
    }

    private SigningCredentials GetSigningCredentials() =>
        new(_parameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256);

    #endregion

    public Guid VerifyAccessToken(string token)
    {
        var principal = _jwtHandler.ValidateToken(token, _parameters, out _);
        var userId = principal.FindFirst("id");

        return userId?.Value
                     .Let(Guid.Parse)
               ?? throw new InvalidCredentialException("Не удалось найти идентификатор пользователя в токене");
    }

    #region RefreshToken

    public async Task<(AuthToken Auth, User User)> RefreshTokenAsync(string token,
                                                                     CancellationToken cancellationToken = default)
    {
        var tokenId = ParseRefreshToken(token);

        var user = await _context.Users
                                 .Include(x => x.Tokens)
                                 .Include(x => x.Communications)
                                 .FirstOrDefaultAsync(x => x.Tokens
                                                            .Any(t => t.Value == tokenId), cancellationToken);

        if (user == null)
        {
            throw new RestException("Токен использован или отозван", HttpStatusCode.Forbidden);
        }

        var tokenModel = user.Tokens
                             .First(t => t.Value == tokenId);

        tokenModel.Renew();
        return (GenerateToken(user, tokenModel), user);
    }

    public Guid ParseRefreshToken(string token)
    {
        try
        {
            var principal = _jwtHandler.ValidateToken(token, _parameters, out _);
            var tokenId = principal.FindFirst(JwtRegisteredClaimNames.Jti);

            if (tokenId == null)
            {
                throw new RestException("Не удалось получить идентификатор токена", HttpStatusCode.Unauthorized);
            }

            return Guid.Parse(tokenId.Value);
        }
        catch (Exception e)
        {
            throw new RestException(e.Message, "JwtException", HttpStatusCode.BadRequest, e);
        }
    }

    #endregion

    private void CleanupExpiredTokens(User user) =>
        user.CleanupExpiredTokens(_options.RefreshTokenLifetime);
}