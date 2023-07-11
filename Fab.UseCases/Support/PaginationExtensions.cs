using AltPoint.Filters;
using AutoMapper;
using Fab.UseCases.Support.Aggregations;
using Fab.UseCases.Support.Filters;
using Fab.UseCases.Support.Pagination;

namespace Fab.UseCases.Support;

public static class PaginationExtensions
{
    public static IQueryable<T> WithFilter<T>(this IQueryable<T> query, IFiltersCollection filters,
                                              IFilterRequest request) =>
        FilterProvider.CreateFilterQuery(query, filters, request);

    public static IQueryable<T> WithAggregation<T>(this IQueryable<T> query, string name,
                                                   Aggregation<T>.Builder builder) =>
        AggregationProvider.CreateAggregationQuery(query, Aggregation<T>.Create(name, builder));

    public static IQueryable<T> WithAggregation<T, TAggregation>(this IQueryable<T> query, string name,
                                                                 Aggregation<T>.Builder<TAggregation> builder) =>
        AggregationProvider.CreateAggregationQuery(query, Aggregation<T>.Create(name, builder));

    public static Task<Page<TSource>> PaginateAsync<TSource>(this IQueryable<TSource> query,
                                                             IPaginationRequest request,
                                                             CancellationToken cancellationToken) =>
        PageFactory.CreateAsync(query, request, null, cancellationToken);

    public static Task<Page<TSource>> PaginateAsync<TSource>(this IQueryable<TSource> query,
                                                             IPaginationRequest request,
                                                             Action<PageOptions> optionsBuilder,
                                                             CancellationToken cancellationToken) =>
        PageFactory.CreateAsync(query, request, optionsBuilder, cancellationToken);

    public static Task<Page<TResult>> PaginateAsync<TSource, TResult>(this IQueryable<TSource> query,
                                                                      IPaginationRequest request,
                                                                      IMapper mapper,
                                                                      CancellationToken cancellationToken) =>
        PageFactory.CreateAsync(query, request, mapper.Map<ICollection<TSource>, ICollection<TResult>>, null,
            cancellationToken);
}