using System.Linq.Expressions;

namespace Fab.Utils.Extensions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Combines the first predicate with the second using the logical "and".
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
                                                   Expression<Func<T, bool>> second) =>
        first.Compose<Func<T, bool>>(second, Expression.AndAlso);

    /// <summary>
    /// Combines the first predicate with the second using the logical "or".
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
                                                  Expression<Func<T, bool>> second) =>
        first.Compose<Func<T, bool>>(second, Expression.OrElse);

    /// <summary>
    /// Negates the predicate.
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var negated = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
    }

    /// <summary>
    /// Combines the first expression with the second using the specified merge function.
    /// </summary>
    public static Expression<T> Compose<T>(this LambdaExpression first, LambdaExpression second,
                                           Func<Expression, Expression, Expression> merge)
    {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters
                       .Select((f, i) => new {f, s = second.Parameters[i]})
                       .ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    public static Expression<Func<TFirstParam, TResult>> Combine<TFirstParam, TIntermediate, TResult>(
        this Expression<Func<TFirstParam, TIntermediate>> first,
        Expression<Func<TIntermediate, TResult>> second)
    {
        var param = Expression.Parameter(typeof(TFirstParam));

        var newFirst = first.Body.Replace(first.Parameters[0], param);
        var newSecond = second.Body.Replace(second.Parameters[0], newFirst);

        return Expression.Lambda<Func<TFirstParam, TResult>>(newSecond, param);
    }

    public static Expression<Func<TFirstParam, TResult>> Combine<TFirstParam, TIntermediate, TResult>(
        this Expression<Func<TFirstParam, IEnumerable<TIntermediate>>> first,
        Expression<Func<TIntermediate, TResult>> second)
    {
        var anyMethod = typeof(Enumerable).GetMethods()
                                          .Single(x => x.Name == "Any" && x.GetParameters().Length == 2);
        var genericMethod = anyMethod.MakeGenericMethod(typeof(TIntermediate));

        var param = Expression.Parameter(typeof(TFirstParam));
        var firstBody = first.Body.Replace(first.Parameters[0], param);
        var res = Expression.Call(null, genericMethod, firstBody, second);

        return Expression.Lambda<Func<TFirstParam, TResult>>(res, param);
    }

    private static Expression Replace(this Expression expression, Expression searchEx, Expression replaceEx) =>
        new ReplaceVisitor(searchEx, replaceEx).Visit(expression);

    private class ReplaceVisitor : ExpressionVisitor
    {
        private readonly Expression _from, _to;

        public ReplaceVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        public override Expression Visit(Expression? node) =>
            (node == _from
                ? _to
                : base.Visit(node))!;
    }

    private class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression>? map) =>
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

        public static Expression ReplaceParameters(Expression exp, ParameterExpression f, ParameterExpression s)
        {
            var map = new Dictionary<ParameterExpression, ParameterExpression>
            {
                {f, s}
            };

            return new ParameterRebinder(map).Visit(exp);
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression>? map,
                                                   Expression exp) =>
            new ParameterRebinder(map).Visit(exp);

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}