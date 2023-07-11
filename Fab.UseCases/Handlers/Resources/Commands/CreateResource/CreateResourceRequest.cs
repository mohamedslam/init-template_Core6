using Fab.UseCases.Handlers.Resources.Dto;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Resources.Commands.CreateResource;

public class CreateResourceRequest : IRequest<Guid>
{
    /// <summary>
    ///     Название ресурса
    /// </summary>
    public string Name { get; set; } = null!;

    [NotMapped]
    public FileInfoDto File { get; set; } = null!;
}