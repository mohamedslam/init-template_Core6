namespace Fab.Entities.Enums.Tenants;

/// <summary>
///     Статус заявки на регистрацию
///     - `Owner` — Владелец тенанта
///     - `Constructor` — Контруктор
///     - `Inspector` — Инспектор
///     - `Viewer` — Просмотрщик
/// </summary>
public enum TenantsRole
{
    Owner,
    Constructor,
    Inspector,
    Viewer
}