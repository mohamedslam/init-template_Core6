using MediatR;

namespace Fab.UseCases.Handlers.Projects.Commands.LoadProjectModel;

public class LoadProjectModelRequest : IRequest<Guid>
{
    public string File { get; set; } = null!;
}