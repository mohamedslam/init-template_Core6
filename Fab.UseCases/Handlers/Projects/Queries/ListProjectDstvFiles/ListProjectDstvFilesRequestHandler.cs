using AltPoint.Filters;
using AutoMapper;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Projects.Dto;
using Fab.UseCases.Support.Pagination;
using MediatR;

namespace Fab.UseCases.Handlers.Projects.Queries.ListProjectDstvFiles;

public class ListProjectDstvFilesRequestHandler : IRequestHandler<ListProjectDstvFilesRequest, Page<ProjectDstvFilesDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListProjectDstvFilesRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public Task<Page<ProjectDstvFilesDto>> Handle(ListProjectDstvFilesRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}