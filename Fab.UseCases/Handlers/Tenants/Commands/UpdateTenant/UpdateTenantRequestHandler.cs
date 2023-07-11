using AutoMapper;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tenants.Commands.UpdateTenant;

public class UpdateTenantRequestHandler : IRequestHandler<UpdateTenantRequest>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateTenantRequestHandler(IMapper mapper, IDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await _dbContext.Tenants
                           .WithScope(request.Scope)
                           .ById(request.TenantId)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Тенант не найден");

        _mapper.Map(request, tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}