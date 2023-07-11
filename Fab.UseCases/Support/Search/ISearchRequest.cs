namespace Fab.UseCases.Support.Search;

public interface ISearchRequest
{
    /// <summary>
    ///     Строка поиска
    /// </summary>
    public string? Query { get; }
}