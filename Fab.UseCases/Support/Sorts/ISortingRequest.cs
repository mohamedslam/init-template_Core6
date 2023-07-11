namespace Fab.UseCases.Support.Sorts;

public interface ISortingRequest
{
    /// <summary>
    ///     Поле для сортировки
    /// </summary>
    public string? SortBy { get; }

    /// <summary>
    ///     Направление сортировки
    /// </summary>
    public SortDirection? SortDir { get; }
}