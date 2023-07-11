using Fab.UseCases.Handlers.Customers.Commands.CreateCustomer;
using Fab.UseCases.Handlers.Customers.Commands.DeleteCustomer;
using Fab.UseCases.Handlers.Customers.Commands.UpdateCustomer;
using Fab.UseCases.Handlers.Customers.Dto;
using Fab.UseCases.Handlers.Customers.Queries.ListCustomers;
using Fab.UseCases.Handlers.Customers.Queries.ReadCustomer;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Fab.Web.Controllers;

/// <summary>
///     Заказчики
/// </summary>
[Route("v{version:apiVersion}/customers")]
[ApiController, ApiVersion("1")]
[Consumes(MediaTypeNames.Application.Json), Produces(MediaTypeNames.Application.Json)]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;


    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    ///     Листинг моделей Customer
    /// </summary>
    [HttpGet]
    public Task<Page<CustomerDto>> ListCustomers([FromQuery] ListCustomersRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Создание модели Customer
    /// </summary>
    [HttpPost]
    public Task<Guid> CreateCustomer([FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    ///     Чтение модели Customer
    /// </summary>
    [HttpGet]
    [Route("{customerId:guid}")]
    public Task<CustomerDto> ReadCustomer([FromRoute] Guid customerId,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new ReadCustomerRequest()
        {
            CustomerId = customerId
        }, cancellationToken);
    }

    /// <summary>
    ///     Обновление модели Customer
    /// </summary>
    [HttpPatch]
    [Route("{customerId:guid}")]
    public Task UpdateCustomer([FromRoute] Guid customerId,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(request.Also(x => x.CustomerId = customerId), cancellationToken);
    }

    /// <summary>
    ///     Удаление модели Customer
    /// </summary>
    [HttpDelete]
    [Route("{customerId:guid}")]
    public Task DeleteCustomer([FromRoute] Guid customerId,
        CancellationToken cancellationToken)
    {
        return _mediator.Send(new DeleteCustomerRequest()
        {
            CustomerId = customerId
        }, cancellationToken);
    }
}