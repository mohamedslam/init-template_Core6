using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Tenants.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tenants.Queries.ListTenants;

public class ListTenantsRequestHandler : IRequestHandler<ListTenantsRequest, Page<TenantDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListTenantsRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<TenantDto>> Handle(ListTenantsRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Tenants
            .AsNoTracking()
            .WithScope(request.Scope)
            .WithFilter(_filters, request)
            .ProjectTo<TenantDto>(_mapper.ConfigurationProvider)
            .PaginateAsync(request, cancellationToken);
    
}