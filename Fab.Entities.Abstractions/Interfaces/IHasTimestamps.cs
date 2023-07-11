namespace Fab.Entities.Abstractions.Interfaces;

public interface IHasTimestamps
{
    /// <summary>
    ///     Дата создания модели
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Дата обновления модели
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}