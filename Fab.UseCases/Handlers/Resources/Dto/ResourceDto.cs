using Fab.Entities.Abstractions.Interfaces;

namespace Fab.UseCases.Handlers.Resources.Dto;

public class ResourceDto
{
    /// <summary>
    ///     ID ресурса
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Название ресурса
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Mime-type файла
    /// </summary>
    public string MimeType { get; set; } = null!;

    /// <summary>
    ///     Вес файла в байтах
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    ///     ID пользователя, создавшего ресурс
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }

    /// <inheritdoc cref="ISoftDeletes.DeletedAt"/>
    public DateTime? DeletedAt { get; set; }
}