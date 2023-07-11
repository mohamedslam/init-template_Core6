using Fab.Utils.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Fab.UseCases.Support;

public class ExpressionUtils
{
    public class ConditionalExtractor : ExpressionVisitor
    {
        private readonly Func<Expression, bool> _condition;
        public Expression? Result { get; private set; }

        private ConditionalExtractor(Func<Expression, bool> condition) =>
            _condition = condition;

        public static Expression? FromExpression(Expression? expression, Func<Expression, bool> condition) =>
            new ConditionalExtractor(condition).Also(x => x.Visit(expression))
                                               .Result;

        public override Expression? Visit(Expression? node)
        {
            if (node != null && _condition(node))
            {
                Result = node;
            }

            return base.Visit(node);
        }
    }

    public class Patcher : ExpressionVisitor
    {
        private readonly Func<Expression, bool> _condition;
        private readonly Func<Expression, Expression?> _patcher;

        private Patcher(Func<Expression, bool> condition, Func<Expression, Expression?> patcher)
        {
            _condition = condition;
            _patcher = patcher;
        }

        public static Expression? FromExpression(Expression? expression, Func<Expression, bool> condition,
                                                 Func<Expression, Expression?> patcher) =>
            new Patcher(condition, patcher).Visit(expression);

        public static IQueryable<T>? FromQuery<T>(IQueryable<T> query, Func<Expression, bool> condition,
                                                  Func<Expression, Expression?> patcher) =>
            FromExpression(query.Expression, condition, patcher)?.Let(query.Provider.CreateQuery<T>);

        public override Expression? Visit(Expression? node) =>
            node != null && _condition(node)
                ? _patcher(node)
                : base.Visit(node);
    }

    public class MultiplePatcher
    {
        public static Expression? FromExpression(Expression? expression, Func<Expression, bool> condition,
                                                 Func<Expression, Expression?> patcher)
        {
            for (var processAggregations = true; processAggregations;)
            {
                processAggregations = false;
                expression = Patcher.FromExpression(expression, condition, expr =>
                {
                    processAggregations = true;
                    return patcher(expr);
                });
            }

            return expression;
        }

        public static IQueryable<T>? FromQuery<T>(IQueryable<T> query, Func<Expression, bool> condition,
                                                  Func<Expression, Expression?> patcher) =>
            FromExpression(query.Expression, condition, patcher)?.Let(query.Provider.CreateQuery<T>);
    }

    public class BeforeProjection
    {
        [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
        public class Patcher
        {
            public static Expression? FromExpression(Expression? expression, Func<Expression, Expression> patcher)
            {
                var isPatched = false;

                var result = ExpressionUtils.Patcher.FromExpression(
                    expression,
                    expr => expr is MethodCallExpression mce && mce.Method.Name == nameof(Queryable.Select),
                    expr => expr is MethodCallExpression mce
                        ? mce.Update(
                                 mce.Object,
                                 mce.Arguments
                                    .Skip(1)
                                    .Prepend(patcher(mce.Arguments.First())))
                             .Also(_ => isPatched = true)
                        : expr);

                return isPatched || result == null
                    ? result
                    : patcher(result);
            }

            public static IQueryable<T>? FromQuery<T>(IQueryable<T> query, Func<Expression, Expression> patcher) =>
                FromExpression(query.Expression, patcher)?.Let(query.Provider.CreateQuery<T>);
        }

        public class Extractor : ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression node) =>
                node.Method.Name != nameof(Queryable.Select)
                    ? base.VisitMethodCall(node)
                    : node.Arguments
                          .First();

            public static Expression? FromExpression(Expression? expression) =>
                new Extractor().Visit(expression);

            public static IQueryable? FromQuery<T>(IQueryable<T> query) =>
                FromExpression(query.Expression)?.Let(query.Provider.CreateQuery<T>);
        }
    }

    public class Enumerator : ExpressionVisitor, IEnumerable<Expression>
    {
        private readonly Expression _expression;
        private readonly bool _reversed;
        private readonly List<Expression> _results = new();

        private Enumerator(Expression expression, bool reverse = false)
        {
            _expression = expression;
            _reversed = reverse;
        }

        public static Enumerator Enumerate(Expression expression, bool reverse = false) => new(expression, reverse);

        public override Expression? Visit(Expression? node)
        {
            if (node is null)
            {
                return null;
            }

            if (!_reversed)
            {
                _results.Add(node);
            }

            base.Visit(node);

            if (_reversed)
            {
                _results.Add(node);
            }

            return node;
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            if (_results.Count == 0)
            {
                Visit(_expression);
            }

            return _results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}