using Fab.Entities.Enums.Users;
using Fab.UseCases.Dto;
using Fab.UseCases.Handlers.Users.Commands.BlockUser;
using Fab.UseCases.Handlers.Users.Commands.ChangeUserRole;
using Fab.UseCases.Handlers.Users.Commands.CreateUserCommunication;
using Fab.UseCases.Handlers.Users.Commands.DeleteUser;
using Fab.UseCases.Handlers.Users.Commands.DeleteUserCommunication;
using Fab.UseCases.Handlers.Users.Commands.UnblockUser;
using Fab.UseCases.Handlers.Users.Commands.UpdateUser;
using Fab.UseCases.Handlers.Users.Dto;
using Fab.UseCases.Handlers.Users.Queries.ListUserCommunications;
using Fab.UseCases.Handlers.Users.Queries.ListUsers;
using Fab.UseCases.Handlers.Users.Queries.ReadUser;
using Fab.UseCases.Handlers.Users.Queries.ReadUserCommunication;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Fab.UseCases.Handlers.Users.Commands.VerifyUserCommunication;

namespace Fab.Web.Controllers;

/// <summary>
///     Пользователи
/// </summary>
[Route("v{version:apiVersion}/users")]
[ApiController, ApiVersion("1")]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) =>
        _mediator = mediator;

    #region User

    /// <summary>
    ///     Листинг пользователей
    /// </summary>
    [HttpGet]
    public Task<Page<UserDto>> ListUsers([FromQuery] ListUsersRequest request,
                                         CancellationToken cancellationToken) =>
        _mediator.Send(request, cancellationToken);

    /// <summary>
    ///     Чтение модели пользователя
    /// </summary>
    [HttpGet]
    [Route("{userId:guid}")]
    public Task<UserDto> ReadUser([FromRoute] Guid userId, CancellationToken cancellationToken) =>
        _mediator.Send(new ReadUserRequest
        {
            UserId = userId
        }, cancellationToken);

    /// <summary>
    ///     Обновление модели пользователя
    /// </summary>
    [HttpPatch]
    [Route("{userId:guid}")]
    public Task UpdateUser([FromRoute] Guid userId,
                           [FromBody] UpdateUserRequest request,
                           CancellationToken cancellationToken) =>
        _mediator.Send(request.Also(x => x.UserId = userId), cancellationToken);

    /// <summary>
    ///     Удаление модели пользователя
    /// </summary>
    [HttpDelete]
    [Route("{userId:guid}")]
    public Task DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken) =>
        _mediator.Send(new DeleteUserRequest
        {
            UserId = userId
        }, cancellationToken);

    /// <summary>
    ///     Блокировка пользователя
    /// </summary>
    [HttpPatch]
    [Route("{userId:guid}/block")]
    public Task BlockUser([FromRoute] Guid userId, CancellationToken cancellationToken) =>
        _mediator.Send(new BlockUserRequest
        {
            UserId = userId
        }, cancellationToken);

    /// <summary>
    ///     Разблокировка пользователя
    /// </summary>
    [HttpPatch]
    [Route("{userId:guid}/un-block")]
    public Task UnblockUser([FromRoute] Guid userId, CancellationToken cancellationToken) =>
        _mediator.Send(new UnblockUserRequest
        {
            UserId = userId
        }, cancellationToken);

    /// <summary>
    ///     Смена роли пользователя
    /// </summary>
    [HttpPut]
    [Route("{userId:guid}/role/{role}")]
    public Task ChangeUserRole([FromRoute] Guid userId,
                               [FromRoute] Role role,
                               CancellationToken cancellationToken) =>
        _mediator.Send(new ChangeUserRoleRequest
        {
            UserId = userId,
            Role = role
        }, cancellationToken);
    #endregion

    #region Communications

    /// <summary>
    ///     Листинг коммуникаций пользователя
    /// </summary>
    [HttpGet]
    [Route("{userId:guid}/communications")]
    public Task<Page<CommunicationDto>> ListUserCommunications([FromQuery] ListUserCommunicationsRequest request,
                                                               [FromRoute] Guid userId,
                                                               CancellationToken cancellationToken) =>
        _mediator.Send(request.Also(x => x.UserId = userId), cancellationToken);

    /// <summary>
    ///     Создание коммуникации пользователя
    /// </summary>
    [HttpPost]
    [Route("{userId:guid}/communications")]
    public Task<Guid> CreateUserCommunication([FromRoute] Guid userId,
                                              [FromBody] CreateUserCommunicationRequest request,
                                              CancellationToken cancellationToken) =>
        _mediator.Send(request.Also(x => x.UserId = userId), cancellationToken);

    /// <summary>
    ///     Чтение коммуникации пользователя
    /// </summary>
    [HttpGet]
    [Route("{userId:guid}/communications/{communicationId:guid}")]
    public Task<CommunicationDto> ReadUserCommunication([FromRoute] Guid userId,
                                                        [FromRoute] Guid communicationId,
                                                        CancellationToken cancellationToken) =>
        _mediator.Send(new ReadUserCommunicationRequest
        {
            UserId = userId,
            CommunicationId = communicationId
        }, cancellationToken);

    /// <summary>
    ///     Запрос одноразового кода подтверждения
    /// </summary>
    [HttpPost]
    [Route("{userId:guid}/communications/{communicationId:guid}/confirm-request")]
    public Task SendVerifyCode([FromRoute] Guid userId,
        [FromRoute] Guid communicationId,
        [FromBody] SendVerifyCodeRequest request,
        CancellationToken cancellationToken) =>
        _mediator.Send(request.Also(r =>
        {
            r.UserId = userId;
            r.CommunicationId = communicationId;
        }), cancellationToken);
    
    /// <summary>
    ///     Подтверждение Communication
    /// </summary>
    [HttpPost]
    [Route("{userId:guid}/communications/{communicationId:guid}/confirm")]
    public Task VerifyUserCommunication([FromRoute] Guid userId,
                                        [FromRoute] Guid communicationId,
                                        [FromBody] VerifyUserCommunicationRequest request,
                                        CancellationToken cancellationToken) =>
        _mediator.Send(request.Also(r =>
        {
            r.UserId = userId;
            r.CommunicationId = communicationId;
        }), cancellationToken);

    /// <summary>
    ///     Удаление коммуникации пользователя
    /// </summary>
    [HttpDelete]
    [Route("{userId:guid}/communications/{communicationId:guid}")]
    public Task DeleteUserCommunication([FromRoute] Guid userId,
                                        [FromRoute] Guid communicationId,
                                        CancellationToken cancellationToken) =>
        _mediator.Send(new DeleteUserCommunicationRequest
        {
            UserId = userId,
            CommunicationId = communicationId
        }, cancellationToken);

    #endregion
}