using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Dto;
using Fab.UseCases.Handlers.Authentication.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignIn;

public class SignInRequest : IRequest<AuthTokenDto>, ITokenInfo
{
     
    /// <summary>
    ///     Коммуникация
    /// </summary>
    public CommunicationShortDto Communication { get; set; } = null!;
    
    [NotMapped]
    public IPAddress IpAddress { get; set; } = null!;

    [NotMapped]
    public string UserAgent { get; set; } = null!;

    /// <summary>
    /// Пароль
    /// </summary>
    public string Password { get; set; } = null!;
}