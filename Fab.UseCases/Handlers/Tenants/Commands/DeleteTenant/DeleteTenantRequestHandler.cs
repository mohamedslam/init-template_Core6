using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tenants.Commands.DeleteTenant;

public class DeleteTenantRequestHandler : IRequestHandler<DeleteTenantRequest>
{
    private readonly IDbContext _dbContext;

    public DeleteTenantRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;


    public async Task<Unit> Handle(DeleteTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _dbContext.Tenants
                         .WithScope(request.Scope)
                         .ById(request.TenantId)
                         .FirstOrDefaultAsync(cancellationToken)
                     ?? throw new NotFoundException("Тенант не найден");

        _dbContext.Remove(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}