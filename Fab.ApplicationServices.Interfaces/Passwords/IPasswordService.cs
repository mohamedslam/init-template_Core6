using Fab.Entities.Models.Communications;
using Fab.Infrastructure.Interfaces.Notifications;

namespace Fab.ApplicationServices.Interfaces.Passwords;

public interface IPasswordService
{
    /// <summary>
    ///     Генерирует код для смены пароля с помощью <see cref="IPasswordCodeGenerator"/>
    ///     и отправляет его с помощью <see cref="INotificationService"/>
    /// </summary>
    /// <param name="communication">Коммуникация</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SendPasswordResetCodeAsync(Communication communication, CancellationToken cancellationToken);
    
    /// <summary>
    ///     Проверяет код подтверждения для смены пароля
    ///     Если код правильный, то изменяет пароль и удаляет использованный код
    /// </summary>
    /// <param name="communication">Коммуникация</param>
    /// <param name="code">Код подтверждения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <exception cref="ArgumentException">Код подтверждения не верный</exception>
    Task ChangePasswordAsync(Communication communication, string code, CancellationToken cancellationToken);
}