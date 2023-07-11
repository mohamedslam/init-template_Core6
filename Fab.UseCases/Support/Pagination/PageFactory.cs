using Fab.UseCases.Support.Aggregations;
using Fab.UseCases.Support.Filters;
using Fab.UseCases.Support.Search;
using Fab.UseCases.Support.Sorts;
using Fab.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Support.Pagination;

public static class PageFactory
{
    private static readonly ICollection<IListingFeatureProvider> Providers = new IListingFeatureProvider[]
    {
        new SortingProvider(),
        new FilterProvider(),
        new SearchProvider(),
        new AggregationProvider(),
        new PaginationProvider()
    };

    public static async Task<Page<TResult>> CreateAsync<TSource, TResult>(
        IQueryable<TSource> query, IPaginationRequest request,
        Func<ICollection<TSource>, ICollection<TResult>> projector,
        Action<PageOptions>? optionsBuilder,
        CancellationToken cancellationToken = default)
    {
        var pageOptions = new PageOptions().Also(opts => optionsBuilder?.Invoke(opts));
        var page = new Page<TResult>();

        query = Providers.Aggregate(query, (q, provider) => provider.Apply(q, page, request, pageOptions));

        foreach (var provider in Providers)
        {
            await provider.EnrichPageAsync(query, page, pageOptions, cancellationToken);
        }

        page.Data = page.Total > 0
            ? projector(await query.ToListAsync(cancellationToken))
            : Array.Empty<TResult>();

        return page;
    }

    public static Task<Page<T>> CreateAsync<T>(IQueryable<T> query, IPaginationRequest request,
                                               Action<PageOptions>? optionsBuilder,
                                               CancellationToken cancellationToken = default) =>
        CreateAsync(query, request, _ => _, optionsBuilder, cancellationToken);
}