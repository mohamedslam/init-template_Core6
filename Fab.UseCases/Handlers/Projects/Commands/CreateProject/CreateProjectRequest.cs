using MediatR;

namespace Fab.UseCases.Handlers.Projects.Commands.CreateProject;

public class CreateProjectRequest : IRequest<Guid>
{
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