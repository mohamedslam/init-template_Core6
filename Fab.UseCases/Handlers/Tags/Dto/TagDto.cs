using Fab.Entities.Abstractions.Interfaces;

namespace Fab.UseCases.Handlers.Tags.Dto;

/// <summary>
///     Тег
/// </summary>
public class TagDto
{
    /// <summary>
    ///     ID тега
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Заголовок тега
    /// </summary>
    public string Label { get; set; } = null!;

    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }
}