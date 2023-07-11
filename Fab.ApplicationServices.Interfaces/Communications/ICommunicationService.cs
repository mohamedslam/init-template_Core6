using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.Interfaces.Notifications;

namespace Fab.ApplicationServices.Interfaces.Communications;

public interface ICommunicationService
{
    /// <summary>
    ///     Ищет коммуникацию по типу и значению или создаёт, если она не существует
    /// </summary>
    /// <param name="type">Тип коммуникации</param>
    /// <param name="value">Значение</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коммуникация</returns>
    Task<Communication> FindOrCreateCommunicationAsync(CommunicationType type, string value,
                                                       CancellationToken cancellationToken);

    /// <summary>
    ///     Ищет коммуникацию по типу и значению или создаёт, если она не существует
    /// </summary>
    Task<Communication> FindOrCreateCommunicationAsync(CommunicationShort communication,
                                                       CancellationToken cancellationToken);

    /// <summary>
    ///     Ищет коммуникации по типу и значению или создаёт, если они не существуют
    /// </summary>
    Task<ICollection<Communication>> FindOrCreateCommunicationsAsync(
        ICollection<CommunicationShort>? shortCommunications, CancellationToken cancellationToken);

    /// <summary>
    ///     Проверяет, необходимо ли подтверждение для данной коммуникации
    /// </summary>
    bool IsVerificationRequired(Communication communication);

    /// <summary>
    ///     Генерирует код подтверждения с помощью <see cref="IVerificationCodeGenerator"/>
    ///     и отправляет его с помощью <see cref="INotificationService"/>
    /// </summary>
    /// <param name="communication">Коммуникация</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SendVerifyCodeAsync(Communication communication, CancellationToken cancellationToken);

    /// <summary>
    ///     Проверяет код подтверждения коммуникации
    ///     Если код правильный, то помечает коммуникацию как подтвержденную и удаляет использованный код
    /// </summary>
    /// <param name="communication">Коммуникация</param>
    /// <param name="code">Код подтверждения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <exception cref="ArgumentException">Код подтверждения не верный</exception>
    Task VerifyCommunicationAsync(Communication communication, string code, CancellationToken cancellationToken);

    /// <summary>
    ///     Помечает коммуникацию как подтвержденную
    /// </summary>
    /// <param name="communication">Коммуникация</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <exception cref="ArgumentException">Код подтверждения не верный</exception>
    Task VerifyCommunicationAsync(Communication communication, CancellationToken cancellationToken);
}