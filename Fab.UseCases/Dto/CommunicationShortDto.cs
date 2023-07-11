using Fab.Entities.Enums.Communications;

namespace Fab.UseCases.Dto;

public class CommunicationShortDto
{
    /// <inheritdoc cref="CommunicationType"/>
    public CommunicationType Type { get; set; }

    /// <summary>
    ///     Значение коммуникации
    /// </summary>
    public string Value { get; set; } = null!;

    /// <summary>
    ///     Идентификатор устройства
    /// </summary>
    public string? DeviceId { get; set; } = null;
}