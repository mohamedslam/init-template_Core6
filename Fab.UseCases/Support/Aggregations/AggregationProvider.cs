using Fab.UseCases.Support.Pagination;
using Fab.Utils.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Fab.UseCases.Support.Aggregations;

public class AggregationProvider : IListingFeatureProvider
{
    private static string TransformAggregationName(string name) =>
        name.Uncapitalize();

    public IQueryable<T> Apply<T>(IQueryable<T> query, Page page, object request, PageOptions options) =>
        ExpressionUtils.MultiplePatcher.FromQuery(query, IsAggregationExpression,
            expr =>
            {
                if (expr is not MethodCallExpression mce)
                {
                    return expr;
                }

                var baseExpression = mce.Arguments.First();
                var entityType = query.Provider
                                      .CreateQuery(baseExpression)
                                      .ElementType;

                var aggregation = typeof(AggregationProvider).GetMethod(nameof(ExtractAggregation),
                                                                 BindingFlags.Static | BindingFlags.NonPublic)!
                                                             .MakeGenericMethod(entityType)
                                                             .Invoke(null, new object[] { query, mce })!
                                                             .As<Aggregation>();

                var aggregations = GetAggregations(options);
                aggregations[TransformAggregationName(aggregation.Name)] = aggregation;

                return baseExpression;
            })!;

    [SuppressMessage("ReSharper", "NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract")]
    public async Task EnrichPageAsync<T>(IQueryable<T> query, Page page, PageOptions options,
                                         CancellationToken cancellationToken = default)
    {
        var aggregations = GetAggregations(options);

        if (aggregations.Count == 0)
        {
            return;
        }

        page.Aggregations ??= new Dictionary<string, object?>();
        foreach (var (name, aggregation) in aggregations)
        {
            page.Aggregations[name] = await aggregation.GetResult(cancellationToken);
        }
    }

    private static bool IsAggregationExpression(Expression expr) =>
        expr is MethodCallExpression mce &&
        mce.Method.Name == nameof(CreateAggregationQuery);

    private static Aggregation ExtractAggregation<TEntity>(IQueryable baseQuery, MethodCallExpression expression)
    {
        var query = expression.Arguments[0]
                              .Let(x => ExpressionUtils.MultiplePatcher.FromExpression(x, IsAggregationExpression,
                                  expr => expr is MethodCallExpression mce
                                      ? mce.Arguments.First()
                                      : expr)!)
                              .Let(baseQuery.Provider.CreateQuery<TEntity>);

        return expression.Arguments[1]
                         .As<ConstantExpression>().Value!
                         .As<Aggregation<TEntity>>()
                         .Also(x => x.Query = query);
    }

    private static Dictionary<string, Aggregation> GetAggregations(PageOptions options)
    {
        if (!options.Context.ContainsKey(nameof(AggregationProvider)))
        {
            var aggregations = new Dictionary<string, Aggregation>();
            options.Context[nameof(AggregationProvider)] = aggregations;

            return aggregations;
        }

        return options.Context[nameof(AggregationProvider)]
                      .As<Dictionary<string, Aggregation>>();
    }

    public static IQueryable<T> CreateAggregationQuery<T>(IQueryable<T> query, Aggregation<T> aggregation) =>
        query.Provider.CreateQuery<T>(
            Expression.Call(null,
                typeof(AggregationProvider).GetMethod(nameof(CreateAggregationQuery))!
                                           .MakeGenericMethod(typeof(T)),
                query.Expression,
                Expression.Constant(aggregation)));
}