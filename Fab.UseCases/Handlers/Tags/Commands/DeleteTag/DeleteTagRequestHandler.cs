using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tags.Commands.DeleteTag;

public class DeleteTagRequestHandler : IRequestHandler<DeleteTagRequest>
{
    private readonly IDbContext _dbContext;

    public DeleteTagRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Unit> Handle(DeleteTagRequest request, CancellationToken cancellationToken)
    {
        var tag = await _dbContext.Tags
                                  .WithScope(request.Scope)
                                  .ById(request.TagId)
                                  .FirstOrDefaultAsync(cancellationToken)
                  ?? throw new NotFoundException("Тег не найден");

        _dbContext.Remove(tag);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}