using Fab.UseCases.Handlers.Authentication.Commands.ChangePassword;
using Fab.UseCases.Handlers.Authentication.Commands.RefreshToken;
using Fab.UseCases.Handlers.Authentication.Commands.ResetPasswordRequest;
using Fab.UseCases.Handlers.Authentication.Commands.SendCode;
using Fab.UseCases.Handlers.Authentication.Commands.SignIn;
using Fab.UseCases.Handlers.Authentication.Commands.SignUp;
using Fab.UseCases.Handlers.Authentication.Commands.SignUpConfirm;
using Fab.UseCases.Handlers.Authentication.Dto;
using Fab.Utils.Extensions;
using Fab.Web.Attributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;

namespace Fab.Web.Controllers;

/// <summary>
///     Аутентификация
/// </summary>
[Route("v{version:apiVersion}/auth")]
[ApiController, ApiVersionNeutral, AllowAnonymous, AllowUnapprovedUsers]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) =>
        _mediator = mediator;

        /// <summary>
    ///     Аутентификация пользователя по коммуникации и коду подтверждения
    /// </summary>
    /// <remarks>
    ///     Проверка одноразового кода, который был выслан на выбранную коммуникацию, и выдача токена доступа в случае успеха
    /// </remarks>
    /// <response code="400">Неверный код подтверждения</response>
    /// <response code="404">Неизвестная коммуникация</response>
    [HttpPost]
    [Route("Sign-in")]
    public Task<AuthTokenDto> SignIn([FromBody] SignInRequest request,
                                     CancellationToken cancellationToken) =>
        _mediator.Send(request.Also(x =>
        {
            x.IpAddress = HttpContext.Request.Headers.ContainsKey("X-Forwarded-For")
                 ? IPAddress.Parse(HttpContext.Request.Headers["X-Forwarded-For"])
                            : HttpContext.Request.Headers.ContainsKey("X-Real-IP")
                 ? IPAddress.Parse(HttpContext.Request.Headers["X-Real-IP"])
                            : HttpContext.Connection.RemoteIpAddress!;
            x.UserAgent = Request.Headers.UserAgent;
        }), cancellationToken);

    /// <summary>
    ///     Регистрация
    /// </summary>
    [HttpPost]
    public Task<Guid> SignUp([FromBody] SignUpRequest request,
                             CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);
    /// <summary>
    ///     Подтверждение регистрации пользователя
    /// </summary>
    [HttpPost]
    [Route("confirm")]
    public Task<Unit> SignUpConfirm([FromBody] SignUpConfirmRequest request,
                             CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);

    /// <summary>
    ///     Обновление access-token с помощью refresh-token
    /// </summary>
    /// <response code="400">Неправильный или уже использованный refresh-token</response>
    [HttpPost]
    [Route("tokens/{refreshToken}/refresh")]
    public Task<AuthTokenDto> RefreshToken([FromRoute] string refreshToken,
                                           CancellationToken cancellationToken) =>
        _mediator.Send(new RefreshTokenRequest
        {
            RefreshToken = refreshToken
        }, cancellationToken);

    /// <summary>
    /// Запрос на изменение пароля пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("reset-password-request")]
    public Task<Guid> ResetPassword([FromBody] ResetPasswordRequest request, 
                                    CancellationToken cancellationToken) =>
          _mediator.Send(request, cancellationToken);

    /// <summary>
    /// Установка нового пароля
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("change-password")]
    public Task<Unit> ChangePassword([FromBody] ChangePasswordRequest request, 
                                    CancellationToken cancellationToken) =>
          _mediator.Send(request, cancellationToken);
}