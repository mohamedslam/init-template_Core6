using AltPoint.Filters;
using AutoMapper;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Projects.Dto;
using Fab.UseCases.Support.Pagination;
using MediatR;

namespace Fab.UseCases.Handlers.Projects.Queries.ListProjectModelsMeta;

public class ListProjectModelsMetaRequestHandler : IRequestHandler<ListProjectModelsMetaRequest, Page<ProjectModelDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListProjectModelsMetaRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public Task<Page<ProjectModelDto>> Handle(ListProjectModelsMetaRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}