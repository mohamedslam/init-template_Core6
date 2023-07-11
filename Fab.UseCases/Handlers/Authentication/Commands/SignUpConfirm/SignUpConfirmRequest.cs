using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignUpConfirm;

public class SignUpConfirmRequest:IRequest 
{
    /// <summary>
    /// 
    /// </summary>
    [Required(ErrorMessage = "должны ввести CommunicationID")]
    public Guid CommunicationId { get; set; } 

    /// <summary>
    /// 
    /// </summary>
    public string Code { get; set; } = null;
}
