using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Communications;

namespace Fab.UseCases.Dto;

public class CommunicationDto
{
    /// <summary>
    ///     ID коммуникации
    /// </summary>
    public Guid Id { get; set; }

    /// <inheritdoc cref="CommunicationType"/>
    public CommunicationType Type { get; set; }
        
    /// <summary>
    ///     Значение коммуникации
    /// </summary>
    public string Value { get; set; } = null!;

    /// <summary>
    ///     Идентификатор устройства
    ///
    ///     Гарантируется что в рамках одного типа для одного устройства может существовать только одна коммуникация
    /// </summary>
    public string? DeviceId { get; set; } = null;
    
    /// <summary>
    ///     Подтверждена пользователем
    /// </summary>
    public bool Confirmed { get; set; }

    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }
}