using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Projects.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Projects.Queries.ListProjects;

public class ListProjectsRequestHandler : IRequestHandler<ListProjectsRequest, Page<ProjectDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListProjectsRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<ProjectDto>> Handle(ListProjectsRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Projects
            .AsNoTracking()
            .WithScope(request.Scope)
            .WithFilter(_filters, request)
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .PaginateAsync(request, cancellationToken);
}