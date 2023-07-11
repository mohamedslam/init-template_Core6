using MediatR;

namespace Fab.UseCases.Handlers.Customers.Commands.CreateCustomer;

public class CreateCustomerRequest : IRequest<Guid>
{
    /// <summary>
    ///     Название заказчика
    /// </summary>
    public string Label { get; set; } = null!;
}