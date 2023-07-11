using Fab.UseCases.Handlers.Authentication.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Authentication.Commands.RefreshToken;

public class RefreshTokenRequest : IRequest<AuthTokenDto>
{
    /// <summary>
    ///     Токен обновления
    /// </summary>
    public string RefreshToken { get; set; } = null!;
}