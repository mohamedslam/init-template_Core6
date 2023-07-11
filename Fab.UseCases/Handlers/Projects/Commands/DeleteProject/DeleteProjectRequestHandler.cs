using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Projects.Commands.DeleteProject;

public class DeleteProjectRequestHandler : IRequestHandler<DeleteProjectRequest>
{
    private readonly IDbContext _dbContext;

    public DeleteProjectRequestHandler(IDbContext dbContext) => 
    _dbContext = dbContext;
    

    public async Task<Unit> Handle(DeleteProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
                           .WithScope(request.Scope)
                           .ById(request.ProjectId)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Проект не найден");

        _dbContext.Remove(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}