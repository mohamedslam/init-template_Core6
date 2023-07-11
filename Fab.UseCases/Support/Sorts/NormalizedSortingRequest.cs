using Fab.UseCases.Support.Pagination;

namespace Fab.UseCases.Support.Sorts;

public class NormalizedSortingRequest : IMultipleSortingRequest
{
    public ICollection<Sorting> Sorts { get; }

    public NormalizedSortingRequest(ICollection<Sorting>? sorts, PageOptions options) =>
        Sorts = sorts is { Count: > 0 }
            ? sorts
            : options.DefaultSorts;

    public NormalizedSortingRequest(IMultipleSortingRequest request, PageOptions options)
        : this(request.Sorts, options)
    {
    }

    public NormalizedSortingRequest(ISortingRequest request, PageOptions options)
        : this(!string.IsNullOrWhiteSpace(request.SortBy) &&
               request.SortDir.HasValue
            ? new[]
            {
                new Sorting
                {
                    Field = request.SortBy,
                    Direction = request.SortDir.Value
                }
            }
            : null,
            options)
    {
    }
}