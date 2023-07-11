using AltPoint.Filters;
using AutoMapper;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Projects.Dto;
using Fab.UseCases.Support.Pagination;
using MediatR;

namespace Fab.UseCases.Handlers.Projects.Queries.ListProjectDrawings;

public class ListProjectDrawingsRequestHandler : IRequestHandler<ListProjectDrawingsRequest, Page<ProjectDrawingDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListProjectDrawingsRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public Task<Page<ProjectDrawingDto>> Handle(ListProjectDrawingsRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}