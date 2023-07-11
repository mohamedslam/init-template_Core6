using Fab.Entities.Abstractions;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Utils.Extensions;

namespace Fab.Entities.Specifications;

public static class CommonSpecificationExtensions
{
    public static IQueryable<T> ById<T>(this IQueryable<T> query, Guid id) where T : IEntity =>
        query.Where(x => x.Id == id);

    public static IQueryable<T> WithoutTrashed<T>(this IQueryable<T> query) where T : ISoftDeletes =>
        query.Where(x => x.DeletedAt == null);

    public static IQueryable<T> OnlyTrashed<T>(this IQueryable<T> query) where T : ISoftDeletes =>
        query.Where(x => x.DeletedAt != null);

    public static IQueryable<T> WithScope<T>(this IQueryable<T> query, Spec<T>? scope)
        where T : IEntity
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        return scope?.Let(x => query.Where(x))
               ?? query;
    }
}