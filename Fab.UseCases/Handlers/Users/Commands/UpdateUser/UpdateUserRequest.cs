using System.ComponentModel.DataAnnotations;
using Fab.Entities.Abstractions;
using Fab.Entities.Models.Users;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Users.Commands.UpdateUser;

public class UpdateUserRequest : IRequest, IScopedRequest<User>
{
    [NotMapped]
    public Spec<User>? Scope { get; set; }

    [NotMapped]
    public Guid UserId { get; set; }

    /// <summary>
    ///     Имя
    /// </summary>
    [Required(ErrorMessage = "Имя должно быть заполнено")]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Фамилмя
    /// </summary>
    public string? Surname { get; set; } = null!;

    /// <summary>
    ///     Отчество
    /// </summary>
    public string? Patronymic { get; set; }

}