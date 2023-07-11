using Fab.Entities.Abstractions.Attributes;
using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Fab.UseCases.Support.Search;

public class SearchProvider : IListingFeatureProvider
{
    private static readonly Dictionary<Type, (ParameterExpression Parameter,
            List<(MemberExpression Expression, SearchMode Mode)> Properties)>
        Cache = new();

    public static readonly MethodInfo LikeMethod =
        Assembly.GetEntryAssembly()!
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .SelectMany(x => x.GetExportedTypes())
                .Where(x => x.IsAbstract && x.IsSealed)
                .First(x => x.Name == "NpgsqlDbFunctionsExtensions")
                .GetMethod("ILike", new[] { typeof(DbFunctions), typeof(string), typeof(string) })!;

    private static readonly MethodInfo WhereMethod =
        typeof(Queryable).GetMethods()
                         .First(x => x.Name == nameof(Queryable.Where) &&
                                     x.GetParameters()
                                      .Last()
                                      .ParameterType
                                      .Let(t => t.IsGenericType &&
                                                t.GenericTypeArguments
                                                 .First()
                                                 .GenericTypeArguments
                                                 .Length == 2));

    public IQueryable<T> Apply<T>(IQueryable<T> query, Page page, object request, PageOptions options) =>
        request is ISearchRequest searchRequest
            ? Apply(query, page, new NormalizedSearchRequest(searchRequest))
            : query;

    public Task EnrichPageAsync<T>(IQueryable<T> query, Page page, PageOptions options,
                                   CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    private static IQueryable<T> Apply<T>(IQueryable<T> query, Page page, ISearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return query;
        }

        return ExpressionUtils.BeforeProjection.Patcher.FromQuery(query, expr =>
        {
            var elementType = query.Provider.CreateQuery(expr).ElementType;

            if (!Cache.ContainsKey(elementType))
            {
                ParseEntity(elementType);
            }

            if (Cache[elementType].Properties.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Search is not configured for the {elementType.Name} entity");
            }

            var (parameter, properties) = Cache[elementType];

            return Expression.Call(null,
                WhereMethod.MakeGenericMethod(elementType),
                expr,
                Expression.Lambda(
                    properties.Select(x => MakePropertySearchExpression(x.Expression, request.Query, x.Mode))
                              .Aggregate(Expression.OrElse),
                    parameter));
        })!;
    }

    private static Expression MakePropertySearchExpression(MemberExpression property, string pattern,
                                                           SearchMode searchMode) =>
        Expression.Call(null, LikeMethod,
            Expression.Constant(EF.Functions),
            property,
            Expression.Constant(searchMode switch
            {
                SearchMode.StartsWith => $"{pattern}%",
                SearchMode.Contains => $"%{pattern}%",
                SearchMode.EndsWith => $"%{pattern}",
                _ => throw new ArgumentOutOfRangeException(nameof(searchMode), searchMode,
                    "Unexpected SearchMode value")
            })
        );

    private static List<(MemberExpression Expression, SearchMode Mode)> ParseEntity(Type entity,
        HashSet<Type>? visited = null)
    {
        visited ??= new HashSet<Type>();

        if (visited.Contains(entity))
        {
            return new List<(MemberExpression, SearchMode)>(0);
        }

        if (Cache.ContainsKey(entity))
        {
            return Cache[entity].Properties;
        }

        visited.Add(entity);

        var parameter = Expression.Parameter(entity, entity.Name[0]
                                                           .ToString()
                                                           .ToLower());

        var searchable = (entity.GetCustomAttribute<SearchableAttribute>() != null
                             ? entity.GetProperties()
                                     .Where(x => x.PropertyType == typeof(string))
                                     .Union(entity.GetProperties()
                                                  .Where(x => x.PropertyType != typeof(string) &&
                                                              x.GetCustomAttribute<SearchableAttribute>() != null))
                             : entity.GetProperties()
                                     .Where(x => x.GetCustomAttribute<SearchableAttribute>() != null))
                         .ToList()
                         .Let(s =>
                             s.Where(x => x.PropertyType == typeof(string))
                              .Select(p => (Expression.Property(parameter, p),
                                  p.GetCustomAttribute<SearchableAttribute>()!.Mode))
                              .Union(s.Where(x => x.PropertyType != typeof(string))
                                      .SelectMany(x =>
                                      {
                                          if (x.PropertyType.IsGenericType &&
                                              x.PropertyType
                                               .GetGenericTypeDefinition() == typeof(ICollection<>))
                                          {
                                              throw new NotSupportedException(
                                                  "Search by collections is not supported"); // yet
                                          }

                                          // var elementType = x.PropertyType.GenericTypeArguments.First();

                                          return ParseEntity(x.PropertyType, visited).Select(me =>
                                              ((MemberExpression)ExpressionUtils.Patcher.FromExpression(me.Expression,
                                                  expr => expr is ParameterExpression,
                                                  expr => expr is ParameterExpression
                                                      ? Expression.Property(parameter, x)
                                                      : expr)!, me.Mode));
                                      })))
                         .ToList();

        Cache[entity] = (parameter, searchable);
        return searchable;
    }
}