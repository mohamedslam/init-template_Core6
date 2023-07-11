using Fab.UseCases.Support.Filters;
using Fab.UseCases.Support.Sorts;
using System.Text.Json.Serialization;

namespace Fab.UseCases.Support.Pagination;

public abstract class Page
{
    /// <summary>
    ///     Общее количество элементов без пагинации
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    ///     Текущая выбранная страница
    /// </summary>
    [JsonPropertyName("page")]
    public int PageNumber { get; set; }

    /// <summary>
    ///     Количество элементов на 1 странице
    /// </summary>
    [JsonPropertyName("limit")]
    public int PerPage { get; set; }

    // public int LastPage => PerPage > 1
    //     ? (int)Math.Ceiling(Total / (double)PerPage)
    //     : PageNumber;

    /// <summary>
    ///     Доступные фильтры
    /// </summary>
    [JsonPropertyOrder(int.MaxValue)]
    public ICollection<HateoasFilter> Filters { get; set; } = null!;

    /// <summary>
    ///     Применённые сортировки
    /// </summary>
    public ICollection<Sorting> Sorts { get; set; } = null!;

    [JsonPropertyName("$aggregations")]
    public IDictionary<string, object?> Aggregations { get; set; } = null!;
}

public class Page<T> : Page
{
    /// <summary>
    ///     Фрагмент коллекции по выбранным условиям выборки
    /// </summary>
    [JsonPropertyOrder(int.MaxValue - 100)]
    public ICollection<T> Data { get; set; } = null!;
}