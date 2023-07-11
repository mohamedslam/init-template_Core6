using MediatR;

namespace Fab.UseCases.Handlers.Projects.Commands.LoadProjectModel;

public class LoadProjectModelRequestHandler  : IRequestHandler<LoadProjectModelRequest, Guid>
{
    public Task<Guid> Handle(LoadProjectModelRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}