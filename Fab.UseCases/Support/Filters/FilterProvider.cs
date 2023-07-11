using AltPoint.Filters;
using AltPoint.Filters.Ast.Lexers;
using AltPoint.Filters.Ast.Parsers;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Exceptions;
using Fab.Utils.Extensions;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Fab.UseCases.Support.Filters;

public class FilterProvider : IListingFeatureProvider
{
    private static readonly MethodInfo WhereMethod = typeof(Queryable).GetMethods()
                                                                      .First(x => x.Name == nameof(Queryable.Where) &&
                                                                          x.GetParameters()
                                                                           .Last()
                                                                           .ParameterType
                                                                           .Let(t => t.IsGenericType &&
                                                                               t.GenericTypeArguments
                                                                                   .First()
                                                                                   .GenericTypeArguments
                                                                                   .Length == 2));

    public IQueryable<T> Apply<T>(IQueryable<T> query, Page page,
                                  object request, PageOptions options) =>
        request is IFilterRequest filterRequest
            ? Apply(query, page, filterRequest, options)
            : query;

    public Task EnrichPageAsync<T>(IQueryable<T> query, Page page, PageOptions options,
                                   CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    private static IQueryable<T> Apply<T>(IQueryable<T> query, Page page,
                                          IFilterRequest request, PageOptions options)
    {
        var expression = ExpressionUtils.ConditionalExtractor.FromExpression(query.Expression,
            expr => expr is MethodCallExpression mce &&
                    mce.Method.Name == nameof(CreateFilterQuery));

        if (expression is not MethodCallExpression filterMce ||
            filterMce.Method.Name != nameof(CreateFilterQuery))
        {
            return query;
        }

        try
        {
            var elementType = query.Provider
                                   .CreateQuery(expression)
                                   .ElementType;

            var filtersCollection = filterMce.Arguments[1]
                                             .As<ConstantExpression>().Value!
                                             .As<IFiltersCollection>();

            var filterExpression =
                typeof(FilterProvider).GetMethod(nameof(CreateFilter), BindingFlags.Static | BindingFlags.NonPublic)!
                                      .MakeGenericMethod(elementType)
                                      .Invoke(null, new object?[] { request, filtersCollection, page, options })!
                                      .As<LambdaExpression?>();

            return ExpressionUtils.Patcher.FromQuery(query,
                expr => expr is MethodCallExpression mce &&
                        mce.Method.Name == nameof(CreateFilterQuery),
                expr =>
                {
                    if (expr is not MethodCallExpression mce)
                    {
                        return expr;
                    }

                    if (filterExpression is null)
                    {
                        return mce.Arguments.First();
                    }

                    return Expression.Call(null, WhereMethod.MakeGenericMethod(elementType),
                        mce.Arguments.First(), filterExpression);
                })!;
        }
        catch (TargetInvocationException tie)
            when (tie is { InnerException: FilterTokenizeException or FilterParseException })
        {
            var e = tie.InnerException;
            throw new RestException(e.Message, e.GetType().Name, HttpStatusCode.BadRequest);
        }
        catch (TargetInvocationException tie)
            when (tie is { InnerException: { } })
        {
            var e = tie.InnerException;
            throw new RestException(e.Message, e.GetType().Name, HttpStatusCode.InternalServerError);
        }
        catch (Exception e)
            when (e is FilterTokenizeException or FilterParseException)
        {
            throw new RestException(e.Message, e.GetType().Name, HttpStatusCode.BadRequest);
        }
    }

    private static LambdaExpression? CreateFilter<TEntity>(IFilterRequest request, IFiltersCollection filters,
                                                           Page page, PageOptions options)
    {
        var filter = filters.Build<TEntity>(request.Filter);

        page.Filters = filter.AvailableFilters
                             .Select(f => new HateoasFilter(f))
                             .ToList();

        return filter.Expression;
    }

    public static IQueryable<T> CreateFilterQuery<T>(IQueryable<T> query, IFiltersCollection filters,
                                                     IFilterRequest request) =>
        query.Provider.CreateQuery<T>(
            Expression.Call(null,
                typeof(FilterProvider).GetMethod(nameof(CreateFilterQuery))!
                                      .MakeGenericMethod(typeof(T)),
                query.Expression,
                Expression.Constant(filters),
                Expression.Constant(request)));
}