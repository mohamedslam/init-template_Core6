using System.ComponentModel.DataAnnotations;

namespace Fab.Entities.Enums;

/// <summary>
///     Статусы для подтверждения
///     - `Approved` — Одобрено
///     - `Pending` — На рассмотрении
///     - `Declined` — Отклонено
/// </summary>
public enum ConfirmationStatus
{
    [Display(Name = "На рассмотрении")]
    Pending,

    [Display(Name = "Одобрено")]
    Approved,

    [Display(Name = "Отклонено")]
    Declined
}