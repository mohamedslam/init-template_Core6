using Fab.UseCases.Support.Sorts;

namespace Fab.UseCases.Support.Pagination;

public class PageOptions
{
    public Dictionary<string, object> Context { get; } = new();

    public int? MaxPageItems { get; set; }
    public List<Sorting> DefaultSorts { get; set; } = new()
    {
        Sorting.Default
    };
}