namespace Fab.Infrastructure.Interfaces.Sms;

public class SmsRuOptions
{
    /// <summary>
    ///     Ключ для подключения к API
    /// </summary>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    ///     Имя отправителя
    /// </summary>
    public string Sender { get; set; } = null!;

    /// <summary>
    ///     Режим дебага
    /// </summary>
    /// <remarks>
    ///     Использует соответствую опцию в API
    ///     Сообщения не будут отправлены получателям
    /// </remarks>
    public bool Debug { get; set; }
}