using Fab.Entities.Abstractions.Interfaces;
using Fab.Utils.Extensions;

namespace Fab.UseCases.Support.Sorts;

public class Sorting
{
    public static readonly Sorting Default = new()
    {
        Field = nameof(IHasTimestamps.CreatedAt).Uncapitalize(),
        Direction = SortDirection.Desc,
    };

    /// <summary>
    ///     Поле для сортировки
    /// </summary>
    public string Field { get; set; } = null!;

    /// <summary>
    ///     Направление сортировки
    /// </summary>
    public SortDirection Direction { get; set; }
}