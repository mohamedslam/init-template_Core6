using Fab.UseCases.Dto;
using MediatR;

namespace Fab.UseCases.Handlers.Authentication.Commands.ResetPasswordRequest
{
    public class ResetPasswordRequest : IRequest<Guid>
    {
        /// <summary>
        ///     Коммуникации
        /// </summary>
        public CommunicationShortDto Communication { get; set; } = null!;
    }
}