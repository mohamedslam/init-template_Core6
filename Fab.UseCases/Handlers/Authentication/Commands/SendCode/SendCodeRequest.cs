using Fab.UseCases.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Authentication.Commands.SendCode;

public class SendCodeRequest : IRequest<Guid>
{
    /// <summary>
    ///     Коммуникация
    /// </summary>
    public CommunicationShortDto Communication { get; set; } = null!;
}