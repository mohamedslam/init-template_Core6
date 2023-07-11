using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Resources.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Resources.Queries.ListResources;

public class ListResourcesRequestHandler : IRequestHandler<ListResourcesRequest, Page<ResourceDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListResourcesRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<ResourceDto>> Handle(ListResourcesRequest request,
                                                CancellationToken cancellationToken) =>
        await _dbContext.Resources
                        .AsNoTracking()
                        .WithScope(request.Scope)
                        .WithFilter(_filters, request)
                        .ProjectTo<ResourceDto>(_mapper.ConfigurationProvider)
                        .PaginateAsync(request, cancellationToken);
}