namespace Fab.UseCases.Handlers.Authentication.Dto;

public class AuthTokenDto
{
    /// <summary>
    ///     Токен доступа
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    ///     Токен обсновления
    /// </summary>
    public string RefreshToken { get; set; } = null!;
}