using Fab.Utils.Extensions;

namespace Fab.UseCases.Support.Aggregations;

public class Aggregation<T> : Aggregation
{
    public delegate Task<object?> Builder(IQueryable<T> query,
                                          CancellationToken cancellationToken = default);

    public delegate Task<TAggregation> Builder<TAggregation>(IQueryable<T> query,
                                                             CancellationToken cancellationToken = default);

    public IQueryable<T>? Query { get; set; }
    public Builder AggregationBuilder { get; }

    private Aggregation(string name, Builder aggregationBuilder)
    {
        Name = name;
        AggregationBuilder = aggregationBuilder;
    }

    public static Aggregation<T> Create(string name, Builder aggregationBuilder) =>
        new(name, aggregationBuilder);

    public static Aggregation<T> Create<TAggregation>(string name, Builder<TAggregation> aggregationBuilder) =>
        new(name, (q, ct) => aggregationBuilder.Invoke(q, ct)
                                               .ContinueWith(task => task.Result?.AsOrDefault<object>(),
                                                   TaskContinuationOptions.OnlyOnRanToCompletion));

    public override Task<object?> GetResult(CancellationToken cancellationToken = default)
    {
        if (Query is null)
        {
            throw new ArgumentNullException(nameof(Query), "Query is not set for aggregation");
        }

        return AggregationBuilder.Invoke(Query, cancellationToken);
    }
}

public abstract class Aggregation
{
    public string Name { get; protected set; } = null!;

    public abstract Task<object?> GetResult(CancellationToken cancellationToken = default);
}