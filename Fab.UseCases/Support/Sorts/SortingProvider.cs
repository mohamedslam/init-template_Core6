using Fab.UseCases.Support.Pagination;
using Fab.Utils.Exceptions;
using Fab.Utils.Extensions;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace Fab.UseCases.Support.Sorts;

public class SortingProvider : IListingFeatureProvider
{
    private static readonly Dictionary<(Type, string), LambdaExpression> Cache = new();

    private static readonly MethodInfo OrderByMethod = GetQueryableMethodInfo(nameof(Queryable.OrderBy));
    private static readonly MethodInfo ThenByMethod = GetQueryableMethodInfo(nameof(Queryable.ThenBy));

    private static readonly MethodInfo OrderByDescendingMethod =
        GetQueryableMethodInfo(nameof(Queryable.OrderByDescending));

    private static readonly MethodInfo ThenByDescendingMethod =
        GetQueryableMethodInfo(nameof(Queryable.ThenByDescending));

    public IQueryable<T> Apply<T>(IQueryable<T> query, Page page, object request, PageOptions options) =>
        request switch
        {
            ISortingRequest sortingRequest => Apply(query, page,
                new NormalizedSortingRequest(sortingRequest, options)),

            IMultipleSortingRequest sortingRequest => Apply(query, page,
                new NormalizedSortingRequest(sortingRequest, options)),

            _ => query
        };

    public Task EnrichPageAsync<T>(IQueryable<T> query, Page page, PageOptions options,
                                   CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    private static LambdaExpression MakeKeySelector(Type entity, string key)
    {
        var cacheKey = (entity, key.ToLower());

        if (!Cache.ContainsKey(cacheKey))
        {
            var parameter = Expression.Parameter(entity, "x");
            var property = key.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                              .Aggregate((Expression)parameter, (expr, prop) =>
                              {
                                  var property = expr.Type.GetProperty(prop,
                                      BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                                  if (property == null)
                                  {
                                      var entityName =
                                          $"{entity.Name}{expr.ToString()[(parameter.Name?.Length ?? 1)..]}";
                                      throw new RestException(
                                          $"Unable to sort: entity {entityName} doesn't contain property \"{prop}\"",
                                          nameof(ArgumentOutOfRangeException),
                                          HttpStatusCode.BadRequest);
                                  }

                                  if (property.PropertyType.IsGenericType &&
                                      property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                                  {
                                      throw new RestException("Sort by collections is not supported",
                                          nameof(NotSupportedException),
                                          HttpStatusCode.BadRequest);
                                  }

                                  return Expression.Property(expr, property);
                              });

            Cache[cacheKey] = Expression.Lambda(property, parameter);
        }

        return Cache[cacheKey];
    }

    private static MethodInfo GetSortMethod(SortDirection direction, bool isFirstSort = false) =>
        (direction, isFirstSort) switch
        {
            (SortDirection.Asc, true) => OrderByMethod,
            (SortDirection.Asc, false) => ThenByMethod,
            (SortDirection.Desc, true) => OrderByDescendingMethod,
            (SortDirection.Desc, false) => ThenByDescendingMethod,
            _ => throw new ArgumentOutOfRangeException(nameof(Sorting.Direction), "Unexpected sort direction")
        };

    private static Expression Sort(Type elementType, Expression query, MethodInfo method, string field)
    {
        var selector = MakeKeySelector(elementType, field);
        var keyType = selector.Body.Type;

        return Expression.Call(null, method.MakeGenericMethod(elementType, keyType), query, selector);
    }

    private static IQueryable<T> Apply<T>(IQueryable<T> query, Page page, IMultipleSortingRequest request)
    {
        page.Sorts = request.Sorts ?? Array.Empty<Sorting>();

        if (page.Sorts.Count == 0)
        {
            return query;
        }

        return ExpressionUtils.BeforeProjection.Patcher.FromQuery(query, expression =>
        {
            var elementType = query.Provider.CreateQuery(expression).ElementType;

            var ordered = page.Sorts
                              .First()
                              .Let(sort => Sort(elementType, expression, GetSortMethod(sort.Direction, true),
                                  sort.Field));

            return page.Sorts
                       .Skip(1)
                       .Aggregate(ordered,
                           (expr, sort) => Sort(elementType, expr, GetSortMethod(sort.Direction), sort.Field));
        })!;
    }

    private static MethodInfo GetQueryableMethodInfo(string name) =>
        typeof(Queryable).GetMethods()
                         .First(x => x.Name == name &&
                                     x.GetParameters().Length == 2);
}