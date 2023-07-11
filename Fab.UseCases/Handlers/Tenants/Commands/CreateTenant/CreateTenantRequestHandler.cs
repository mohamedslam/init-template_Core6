using AutoMapper;
using Fab.Entities.Models.Tenants;
using Fab.Infrastructure.DataAccess.Interfaces;
using MediatR;

namespace Fab.UseCases.Handlers.Tenants.Commands.CreateTenant;

public class CreateTenantRequestHandler : IRequestHandler<CreateTenantRequest, Guid>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateTenantRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<Guid> Handle(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = _mapper.Map<Tenant>(request);

        _dbContext.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }
}