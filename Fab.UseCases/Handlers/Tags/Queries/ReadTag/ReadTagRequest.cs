using Fab.Entities.Abstractions;
using Fab.Entities.Models.Tags;
using Fab.UseCases.Handlers.Tags.Dto;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Tags.Queries.ReadTag;

public class ReadTagRequest : IRequest<TagDto>, IScopedRequest<Tag>
{
    [NotMapped]
    public Spec<Tag>? Scope { get; set; }

    [NotMapped]
    public Guid TagId { get; set; }
}