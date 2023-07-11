using Fab.UseCases.Support.Pagination;

namespace Fab.UseCases.Support;

public interface IListingFeatureProvider
{
    IQueryable<T> Apply<T>(IQueryable<T> query, Page page, object request, PageOptions options);
    Task EnrichPageAsync<T>(IQueryable<T> query, Page page, PageOptions options,
                            CancellationToken cancellationToken = default);
}