using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Models.Customers;

namespace Fab.UseCases.Handlers.Projects.Dto;

public class ProjectDto
{
    /// <summary>
    ///     ID Проекта
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// шифр проекта
    /// </summary>
    public string Code { get; set; } = null!;
    
    /// <summary>
    /// Модель заказчика
    /// </summary>
    public Customer Customer { get; set; } = null!;
    
    /// <summary>
    /// название проекта
    /// </summary>
    public string Label { get; set; } = null!;

    /// <summary>
    /// Описание проекта
    /// </summary>
    public string Description { get; set; } = null!;
    
    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }
}