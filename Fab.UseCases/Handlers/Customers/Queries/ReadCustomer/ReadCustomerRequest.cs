using Fab.Entities.Abstractions;
using Fab.Entities.Models.Customers;
using Fab.UseCases.Handlers.Customers.Dto;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Customers.Queries.ReadCustomer;

public class ReadCustomerRequest : IRequest<CustomerDto>, IScopedRequest<Customer>
{
    [NotMapped]
    public Spec<Customer>? Scope { get; set; }

    [NotMapped]
    public Guid CustomerId { get; set; }
}