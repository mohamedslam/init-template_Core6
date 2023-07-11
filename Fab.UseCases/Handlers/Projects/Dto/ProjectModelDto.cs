using Fab.Entities.Abstractions.Interfaces;

namespace Fab.UseCases.Handlers.Projects.Dto;

public class ProjectModelDto
{
    /// <summary>
    ///     ID модели по проекту
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///    Название модели
    /// </summary>
    public string Label { get; set; } = null!;
    
    /// <summary>
    ///    Описание модели
    /// </summary>
    public string Description { get; set; } = null!;
    
    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }
}