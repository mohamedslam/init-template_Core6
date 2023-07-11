using Fab.UseCases.Handlers.Notifications.Dto;
using Fab.UseCases.Support.Filters;
using Fab.UseCases.Support.Pagination;
using Fab.UseCases.Support.Search;
using Fab.UseCases.Support.Sorts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Notifications.Queries.ListNotifications;

public class ListNotificationsRequest : IRequest<Page<NotificationDto>>,
                                        IPaginationRequest, IMultipleSortingRequest, IFilterRequest, ISearchRequest
{
    /// <inheritdoc cref="IPaginationRequest.Page"/>
    public int Page { get; set; }

    /// <inheritdoc cref="IPaginationRequest.Limit"/>
    public int? Limit { get; set; }

    /// <inheritdoc cref="IMultipleSortingRequest.Sorts"/>
    public ICollection<Sorting>? Sorts { get; set; }

    /// <inheritdoc cref="AltPoint.Filters.Filter"/>
    public string? Filter { get; set; }

    /// <inheritdoc cref="DbLoggerCategory.Query"/>
    public string? Query { get; set; }
}