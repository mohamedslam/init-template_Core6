namespace Fab.Infrastructure.Interfaces.Sms;

public interface ISmsService
{
    /// <summary>
    ///     Отправляет sms сообщение
    /// </summary>
    /// <param name="phone">Телефон получателя в формате 79991112233</param>
    /// <param name="content">Текст сообщения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SendAsync(string phone, string content, CancellationToken cancellationToken = default);
}