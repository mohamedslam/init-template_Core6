using Fab.Entities.Abstractions;
using Fab.Entities.Models.Resources;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Resources.Commands.DeleteResource;

public class DeleteResourceRequest : IRequest, IScopedRequest<Resource>
{
    [NotMapped]
    public Spec<Resource>? Scope { get; set; }

    [NotMapped]
    public Guid ResourceId { get; set; }
}