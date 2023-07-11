namespace Fab.UseCases.Support.Filters;

public interface IFilterRequest
{
    /// <summary>
    ///     Spleen фильтр
    /// </summary>
    public string? Filter { get; }
}