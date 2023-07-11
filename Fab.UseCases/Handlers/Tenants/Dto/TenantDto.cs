using Fab.Entities.Abstractions.Interfaces;

namespace Fab.UseCases.Handlers.Tenants.Dto;

public class TenantDto
{
    /// <summary>
    ///     ID тенанта
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///    Название тенанта
    /// </summary>
    public string Label { get; set; } = null!;
    
    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }
}