using AltPoint.Filters;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Fab.UseCases.Support.Filters;

public class HateoasFilter
{
    /// <summary>
    ///     Имя сущности
    /// </summary>
    public string EntityName { get; }

    /// <summary>
    ///     Имя фильтра (поля)
    /// </summary>
    public string FilterName { get; }

    /// <summary>
    ///     Тип элемента
    /// </summary>
    public string ElementType { get; }

    /// <summary>
    ///     Варианты (используется, например, для вывода списка значений `enum`'ов)
    /// </summary>
    public object? Variants { get; }

    /// <summary>
    ///     Признак коллекции
    /// </summary>
    public bool IsCollection { get; }

    /// <summary>
    ///     Признак связи с другой сущностью
    /// </summary>
    public bool IsRelation { get; }

    /// <summary>
    ///     Признак возможного отсутствия значения
    /// </summary>
    public bool IsNullable { get; }

    public HateoasFilter(Filter filter)
    {
        EntityName = filter.EntityType
                           .ShortDisplayName();
        FilterName = filter.FilterName;
        ElementType = filter.ElementType
                            .ShortDisplayName();
        Variants = GetVariants(filter);
        IsNullable = filter.ElementType.IsGenericType &&
                     filter.ElementType.GetGenericTypeDefinition() == typeof(Nullable<>) ||
                     filter.ElementType.IsClass;
        IsCollection = filter.IsCollection;
        IsRelation = filter.IsRelatedEntity;
    }

    private static object? GetVariants(Filter filter)
    {
        if (filter.ElementType.IsEnum)
        {
            return filter.ElementType
                         .GetEnumValues();
        }

        return null;
    }

    private static string GetTypeName(Type type) =>
        type.IsGenericType
            ? $"{type.Name}[{string.Join(", ", type.GenericTypeArguments.Select(GetTypeName))}]"
            : type.Name;
}