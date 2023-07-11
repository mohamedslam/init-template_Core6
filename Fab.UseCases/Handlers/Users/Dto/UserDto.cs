using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Users;
using Fab.UseCases.Dto;

namespace Fab.UseCases.Handlers.Users.Dto;

/// <summary>
///     Пользователь
/// </summary>
public class UserDto
{
    /// <summary>
    ///     ID пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Имя
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Фамилия
    /// </summary>
    public string Surname { get; set; } = null!;

    /// <summary>
    ///     Отчество
    /// </summary>
    public string? Patronymic { get; set; }

    /// <summary>
    ///     Коммуникации пользователя
    /// </summary>
    public ICollection<CommunicationDto> Communications { get; set; } = null!;

    /// <summary>
    ///     Статус активности пользователя
    /// </summary>
    public bool Blocked { get; set; }

    public Role Role { get; set; }

    /// <summary>
    ///     Дата последней аутентификации пользователя
    /// </summary>
    public DateTime LastLoginAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.CreatedAt"/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc cref="IHasTimestamps.UpdatedAt"/>
    public DateTime UpdatedAt { get; set; }
}