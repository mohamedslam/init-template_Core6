namespace Fab.Infrastructure.Interfaces.Email;

public interface IEmailService
{
    /// <summary>
    ///     Отправка email сообщения
    /// </summary>
    /// <param name="email">Email адрес</param>
    /// <param name="subject">Тема</param>
    /// <param name="content">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SendAsync(string email, string subject, string content, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Рассылка email сообщений
    /// </summary>
    /// <param name="emails">Email адреса</param>
    /// <param name="subject">Тема</param>
    /// <param name="content">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SendAsync(IReadOnlyCollection<string> emails, string subject, string content,
                   CancellationToken cancellationToken = default);
}