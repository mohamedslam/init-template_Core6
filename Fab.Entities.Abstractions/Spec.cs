using Fab.Utils.Extensions;
using FastExpressionCompiler;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Fab.Entities.Abstractions;

public class Spec<T>
{
    public static bool operator false(Spec<T> spec) => false;

    public static bool operator true(Spec<T> spec) => true;

    public static Spec<T> operator &(Spec<T> spec1, Spec<T> spec2)
        => new(spec1._expression.And(spec2._expression));

    public static Spec<T> operator &(Spec<T> spec, Expression<Func<T, bool>> expr)
        => new(spec._expression.And(expr));

    public static Spec<T> operator |(Spec<T> spec1, Spec<T> spec2)
        => new(spec1._expression.Or(spec2._expression));

    public static Spec<T> operator |(Spec<T> spec, Expression<Func<T, bool>> expr)
        => new(spec._expression.Or(expr));

    public static Spec<T> operator !(Spec<T> spec)
        => new(spec._expression.Not());

    [return: NotNullIfNotNull("spec")]
    public static implicit operator Expression<Func<T, bool>>?(Spec<T>? spec)
        => spec?._expression;

    public static explicit operator Func<T, bool>(Spec<T> spec)
        => spec._expression.CompileFast();

    public static implicit operator Spec<T>(Expression<Func<T, bool>> expression)
        => new(expression);

    private readonly Expression<Func<T, bool>> _expression;

    public Spec(Expression<Func<T, bool>> expression) => 
        _expression = expression ?? throw new ArgumentNullException(nameof(expression));

    // don't used because compilation of expression takes a long time
    public bool IsSatisfiedBy(T entity) =>
        _expression.Compile()(entity);

    public override string ToString() =>
        _expression.ToCSharpString();
}