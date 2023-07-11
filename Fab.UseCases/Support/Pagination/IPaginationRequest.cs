namespace Fab.UseCases.Support.Pagination;

public interface IPaginationRequest
{
    /// <summary>
    ///     Номер страницы
    /// </summary>
    public int Page { get; }

    /// <summary>
    ///     Записей на странице
    /// </summary>
    public int? Limit { get; }
}