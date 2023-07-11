using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Resources.Commands.DeleteResource;

public class DeleteResourceRequestHandler : IRequestHandler<DeleteResourceRequest>
{
    private readonly IDbContext _dbContext;

    public DeleteResourceRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Unit> Handle(DeleteResourceRequest request, CancellationToken cancellationToken)
    {
        var resource = await _dbContext.Resources
                                       .WithoutTrashed()
                                       .WithScope(request.Scope)
                                       .ById(request.ResourceId)
                                       .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Ресурс не найден");

        _dbContext.Remove(resource);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}