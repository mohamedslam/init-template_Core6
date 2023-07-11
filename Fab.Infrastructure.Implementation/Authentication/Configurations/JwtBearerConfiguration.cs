using Fab.Utils.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Fab.Infrastructure.Implementation.Authentication.Configurations;

public class JwtBearerConfiguration : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly TokenValidationParameters _options;

    public JwtBearerConfiguration(IOptions<TokenValidationParameters> options) =>
        _options = options.Value;

    public void Configure(JwtBearerOptions options)
    {
        options.IncludeErrorDetails = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = _options;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var path = ctx.Request.Path;

                if (path.StartsWithSegments("/hubs"))
                {
                    var token = ctx.Request.Query["access_token"];

                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        ctx.Token = token;
                    }
                }

                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                if (ctx.Response.HasStarted || ctx.Handled)
                {
                    return Task.CompletedTask;
                }

                ctx.HandleResponse();

                throw new RestException(!string.IsNullOrWhiteSpace(ctx.ErrorDescription)
                        ? ctx.ErrorDescription!
                        : ctx.AuthenticateFailure?.Message
                          ?? ctx.Error
                          ?? "Unauthorized",
                    HttpStatusCode.Unauthorized);
            }
        };
    }

    public void Configure(string name, JwtBearerOptions options) =>
        Configure(options);
}