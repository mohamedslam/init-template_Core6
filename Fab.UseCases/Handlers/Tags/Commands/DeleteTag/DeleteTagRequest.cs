using Fab.Entities.Abstractions;
using Fab.Entities.Models.Tags;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Tags.Commands.DeleteTag;

public class DeleteTagRequest : IRequest, IScopedRequest<Tag>
{
    [NotMapped]
    public Spec<Tag>? Scope { get; set; }

    [NotMapped]
    public Guid TagId { get; set; }
}