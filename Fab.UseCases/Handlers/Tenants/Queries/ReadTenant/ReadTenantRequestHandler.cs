using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Tenants.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tenants.Queries.ReadTenant;

public class ReadTenantRequestHandler : IRequestHandler<ReadTenantRequest, TenantDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadTenantRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TenantDto> Handle(ReadTenantRequest request, CancellationToken cancellationToken) =>
    await _dbContext.Tenants
                .AsNoTracking()
                .WithScope(request.Scope)
                .ById(request.TenantId)
                .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Тенант не найден");
    
}