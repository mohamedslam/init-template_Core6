namespace Fab.Entities.Enums.Users;

/// <summary>
///     Статус заявки на регистрацию
///     - `Admin` — Администратор сервиса
///     - `Manager` — Менеджер
///     - `User` — Пользователь
///     - `Viewer` — Просмотрщик
/// </summary>
public enum Role
{
    Admin,
    Manager,
    User,
    Viewer
}