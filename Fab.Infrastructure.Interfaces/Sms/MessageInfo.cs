using System.Text.Json.Serialization;

namespace Fab.Infrastructure.Interfaces.Sms;

public class MessageInfo
{
    /// <summary>
    ///     Идентификатор сообщения
    /// </summary>
    [JsonPropertyName("sms_id")]
    public string Id { get; set; } = null!;

    /// <summary>
    ///     Телефон
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    ///     Статус сообщения
    /// </summary>
    [JsonPropertyName("status_code")]
    public ResponseCode Status { get; set; }

    /// <summary>
    ///     Стоимость отправки
    /// </summary>
    public double? Cost { get; set; }

    public override string ToString() =>
        $"phone={(!string.IsNullOrEmpty(Phone) ? Phone : "?")}, id={(!string.IsNullOrEmpty(Id) ? Id : "?")}, cost={(Cost.HasValue ? Cost.Value : "?")}, status={Status.ToString()}";
}