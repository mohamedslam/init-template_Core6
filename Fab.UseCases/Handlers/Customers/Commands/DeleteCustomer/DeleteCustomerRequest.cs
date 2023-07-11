using Fab.Entities.Abstractions;
using Fab.Entities.Models.Customers;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Customers.Commands.DeleteCustomer;

public class DeleteCustomerRequest : IRequest, IScopedRequest<Customer>
{
    [NotMapped]
    public Spec<Customer>? Scope { get; set; }

    [NotMapped]
    public Guid CustomerId { get; set; }
}