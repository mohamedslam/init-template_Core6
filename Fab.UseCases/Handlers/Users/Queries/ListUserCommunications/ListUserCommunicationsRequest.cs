using Fab.Entities.Abstractions;
using Fab.Entities.Models.Communications;
using Fab.UseCases.Dto;
using Fab.UseCases.Support.Filters;
using Fab.UseCases.Support.Pagination;
using Fab.UseCases.Support.Scopes;
using Fab.UseCases.Support.Sorts;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Users.Queries.ListUserCommunications;

public class ListUserCommunicationsRequest : IRequest<Page<CommunicationDto>>, IScopedRequest<Communication>,
                                             IPaginationRequest, IMultipleSortingRequest, IFilterRequest
{
    [NotMapped]
    public Spec<Communication>? Scope { get; set; }

    [NotMapped]
    public Guid UserId { get; set; }

    /// <inheritdoc cref="IPaginationRequest.Page"/>
    public int Page { get; set; }

    /// <inheritdoc cref="IPaginationRequest.Limit"/>
    public int? Limit { get; set; }

    /// <inheritdoc cref="IMultipleSortingRequest.Sorts"/>
    public ICollection<Sorting>? Sorts { get; set; }

    /// <inheritdoc cref="IFilterRequest.Filter"/>
    public string? Filter { get; set; }
}