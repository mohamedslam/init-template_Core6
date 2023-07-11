using Fab.Entities.Abstractions;
using Fab.Entities.Models.Projects;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Projects.Commands.DeleteProject;

public class DeleteProjectRequest : IRequest, IScopedRequest<Project>
{
    [NotMapped]
    public Spec<Project>? Scope { get; set; }

    [NotMapped]
    public Guid ProjectId { get; set; }
}