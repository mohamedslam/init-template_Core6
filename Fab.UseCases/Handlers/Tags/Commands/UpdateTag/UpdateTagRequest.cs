using Fab.Entities.Abstractions;
using Fab.Entities.Models.Tags;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Tags.Commands.UpdateTag;

public class UpdateTagRequest : IRequest, IScopedRequest<Tag>
{
    [NotMapped]
    public Spec<Tag>? Scope { get; set; }

    [NotMapped]
    public Guid TagId { get; set; }

    /// <summary>
    ///     Заголовок тега
    /// </summary>
    public string Label { get; set; } = null!;
}