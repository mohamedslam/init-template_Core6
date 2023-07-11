using Fab.Entities.Abstractions;
using Fab.Entities.Models.Resources;
using Fab.UseCases.Handlers.Resources.Dto;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Resources.Queries.ReadResource;

public class ReadResourceRequest : IRequest<ResourceDto>, IScopedRequest<Resource>
{
    [NotMapped]
    public Spec<Resource>? Scope { get; set; }

    [NotMapped]
    public Guid ResourceId { get; set; }
}