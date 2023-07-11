using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Authentication.Commands.ChangePassword
{
    public class ChangePasswordRequest : IRequest
    {
        /// <summary>
        /// Новый пароль
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string NewPassword { get; set; } = null!;
        /// <summary>
        /// Код из письма для смены пароля
        /// </summary>
        public string Code { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public Guid CommunicationId { get; set; }
    }
}
