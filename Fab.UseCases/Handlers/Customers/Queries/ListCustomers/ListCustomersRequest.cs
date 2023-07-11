using Fab.Entities.Abstractions;
using Fab.Entities.Models.Customers;
using Fab.UseCases.Handlers.Customers.Dto;
using Fab.UseCases.Support.Filters;
using Fab.UseCases.Support.Pagination;
using Fab.UseCases.Support.Scopes;
using Fab.UseCases.Support.Search;
using Fab.UseCases.Support.Sorts;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Customers.Queries.ListCustomers;

public class ListCustomersRequest : IRequest<Page<CustomerDto>>, IScopedRequest<Customer>,
                                    IPaginationRequest, IMultipleSortingRequest, IFilterRequest, ISearchRequest
{
    [NotMapped]
    public Spec<Customer>? Scope { get; set; }

    /// <inheritdoc cref="IPaginationRequest.Page"/>
    public int Page { get; set; }

    /// <inheritdoc cref="IPaginationRequest.Limit"/>
    public int? Limit { get; set; }

    /// <inheritdoc cref="IMultipleSortingRequest.Sorts"/>
    public ICollection<Sorting>? Sorts { get; set; }

    /// <inheritdoc cref="IFilterRequest.Filter"/>
    public string? Filter { get; set; }

    /// <inheritdoc cref="ISearchRequest.Query"/>
    public string? Query { get; set; }
}