using Fab.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fab.UseCases.Support.Pagination;

public class PaginationProvider : IListingFeatureProvider
{
    private const string CountQueryContextKey = "__QueryWitoutPagination";

    public IQueryable<T> Apply<T>(IQueryable<T> query, Page page, object request, PageOptions options) =>
        request is IPaginationRequest paginationRequest
            ? Apply(query, page, options, new NormalizedPaginationRequest(paginationRequest, options))
            : query;

    public async Task EnrichPageAsync<T>(IQueryable<T> query, Page page, PageOptions options,
                                         CancellationToken cancellationToken = default)
    {
        page.Total = await GetQueryCount(
            options.Context
                   .Let(x => x.Remove(CountQueryContextKey, out var countQuery)
                       ? countQuery.As<IQueryable<T>>()
                       : query),
            cancellationToken);
    }

    private static IQueryable<T> Apply<T>(IQueryable<T> query, Page page, PageOptions options,
                                          IPaginationRequest request)
    {
        page.PerPage = request.Limit ?? page.Total;
        page.PageNumber = request.Limit.HasValue ? request.Page : 1;

        options.Context[CountQueryContextKey] = query;

        if (!request.Limit.HasValue)
        {
            return query;
        }

        return query.Skip(request.Limit.Value * (request.Page - 1))
                    .Take(request.Limit.Value);
    }

    private static async Task<int> GetQueryCount<TSource>(IQueryable<TSource> query,
                                                          CancellationToken cancellationToken)
    {
        try
        {
            return await query.CountAsync(cancellationToken);
        }
        catch
        {
            var baseExpression = ExpressionUtils.BeforeProjection.Extractor.FromExpression(query.Expression);

            if (baseExpression == null)
            {
                throw;
            }

            var baseQuery = query.Provider.CreateQuery(baseExpression);

            var createQuery = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.CreateQuery), 1, new[] { typeof(Expression) })!;

            var countAsync = typeof(EntityFrameworkQueryableExtensions)
                             .GetMethods()
                             .Single(x => x.Name == nameof(EntityFrameworkQueryableExtensions.CountAsync) &&
                                          x.GetParameters().Length == 2);

            var queryWithoutProjection = createQuery.MakeGenericMethod(baseQuery.ElementType)
                                                    .Invoke(query.Provider, new object?[] { baseQuery.Expression });

            var task = (Task<int>)countAsync.MakeGenericMethod(baseQuery.ElementType)
                                            .Invoke(null, new[] { queryWithoutProjection, cancellationToken })!;

            return await task;
        }
    }
}