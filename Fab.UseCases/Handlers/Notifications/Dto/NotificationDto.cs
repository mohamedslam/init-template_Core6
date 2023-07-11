using Fab.Entities.Abstractions.Interfaces;

namespace Fab.UseCases.Handlers.Notifications.Dto;

/// <summary>
///     Уведомление
/// </summary>
public class NotificationDto
{
    /// <summary>
    ///     ID уведомления
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Тип сущности
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    ///     ID сущности
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    ///     Заголовок (берётся из сущности)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     Текстовое описание уведомления
    /// </summary>
    public string Description { get; set; } = null!;

    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Дата прочтения уведомления
    /// </summary>
    public DateTime? ReadAt { get; set; }
}