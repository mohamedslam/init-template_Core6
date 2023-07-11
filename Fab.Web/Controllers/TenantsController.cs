using Fab.UseCases.Handlers.Tenants.Commands.CreateTenant;
using Fab.UseCases.Handlers.Tenants.Commands.DeleteTenant;
using Fab.UseCases.Handlers.Tenants.Commands.UpdateTenant;
using Fab.UseCases.Handlers.Tenants.Dto;
using Fab.UseCases.Handlers.Tenants.Queries.ListTenants;
using Fab.UseCases.Handlers.Tenants.Queries.ReadTenant;
using Fab.UseCases.Handlers.TenatsInvites.Commands.CreateInvite;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Fab.Web.Controllers;

/// <summary>
///     Тенанты
/// </summary>
[Route("v{version:apiVersion}/tenants")]
[ApiController, ApiVersion("1")]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    ///     Листинг моделей Tenant
    /// </summary>
    [HttpGet]
    public Task<Page<TenantDto>> ListCustomers([FromQuery] ListTenantsRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Создание модели Tenant
    /// </summary>
    [HttpPost]
    public Task<Guid> CreateTenant([FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Чтение модели Tenant
    /// </summary>
    [HttpGet]
    [Route("{tenantId:guid}")]
    public Task<TenantDto> ReadTenant([FromRoute] Guid tenantId,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new ReadTenantRequest
        {
            TenantId = tenantId
        }, cancellationToken);
    }

    /// <summary>
    ///     Обновление модели Tenant
    /// </summary>
    [HttpPatch]
    [Route("{tenantId:guid}")]
    public Task UpdateTenant([FromRoute] Guid tenantId,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request.Also(x => x.TenantId = tenantId), cancellationToken);
    }

    /// <summary>
    ///     Удаление модели Tenant
    /// </summary>
    [HttpDelete]
    [Route("{tenantId:guid}")]
    public Task DeleteTenant([FromRoute] Guid tenantId,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new DeleteTenantRequest
        {
            TenantId = tenantId
        }, cancellationToken);
    }

    /// <summary>
    ///     Приглашение пользователя в тенант На адрес электронной почты
    /// </summary>
    [HttpPost]
    [Route("{tenantId:guid}")]
    public Task<Guid> CreateInvitesTenants([FromRoute] Guid tenantId, 
        [FromBody] CreateInviteRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new CreateInviteRequest { 
        TenantId=tenantId,
        Email=request.Email,
        Role=request.Role,
        Password=request.Password
        }, cancellationToken);
    }
}