using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Tags.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Tags.Queries.ListTags;

public class ListTagsRequestHandler : IRequestHandler<ListTagsRequest, Page<TagDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListTagsRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<TagDto>> Handle(ListTagsRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Tags
                        .AsNoTracking()
                        .WithScope(request.Scope)
                        .WithFilter(_filters, request)
                        .ProjectTo<TagDto>(_mapper.ConfigurationProvider)
                        .PaginateAsync(request, cancellationToken);
}