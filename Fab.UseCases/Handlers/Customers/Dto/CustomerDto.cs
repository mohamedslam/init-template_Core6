using Fab.Entities.Abstractions.Interfaces;

namespace Fab.UseCases.Handlers.Customers.Dto;

public class CustomerDto
{
    /// <summary>
    ///     ID заказчика
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///    Название заказчика
    /// </summary>
    public string Label { get; set; } = null!;
    
    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }
}