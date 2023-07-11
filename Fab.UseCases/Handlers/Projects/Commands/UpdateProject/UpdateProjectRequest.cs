using Fab.Entities.Abstractions;
using Fab.Entities.Models.Projects;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Projects.Commands.UpdateProject;

public class UpdateProjectRequest : IRequest, IScopedRequest<Project>
{
    [NotMapped]
    public Spec<Project>? Scope { get; set; }

    [NotMapped]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// шифр проекта
    /// </summary>
    public string Code { get; set; } = null!;
    
    /// <summary>
    /// ID заказчика
    /// </summary>
    public string CustomerId { get; set; } = null!;
    
    /// <summary>
    /// название проекта
    /// </summary>
    public string Label { get; set; } = null!;

    /// <summary>
    /// Описание проекта
    /// </summary>
    public string Description { get; set; } = null!;
}