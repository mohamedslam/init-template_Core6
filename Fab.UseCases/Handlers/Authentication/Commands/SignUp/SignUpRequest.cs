using System.ComponentModel.DataAnnotations;
using Fab.UseCases.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignUp;

public class SignUpRequest : IRequest<Guid>
{
    /// <summary>
    ///  Пароль для пользователя
    /// </summary>
    [Required(ErrorMessage = "Пароль должен быть заполнен")]
    public string Password { get; set; } = null!;

    /// <summary>
    ///     Коммуникации
    /// </summary>
    public ICollection<CommunicationShortDto> Communications { get; set; } = null!;
}