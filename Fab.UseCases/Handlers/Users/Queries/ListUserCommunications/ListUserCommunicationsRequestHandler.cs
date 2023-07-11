using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Queries.ListUserCommunications;

public class ListUserCommunicationsRequestHandler
    : IRequestHandler<ListUserCommunicationsRequest, Page<CommunicationDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListUserCommunicationsRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters,
                                                IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<CommunicationDto>> Handle(ListUserCommunicationsRequest request,
                                                     CancellationToken cancellationToken) =>
        await _dbContext.Users
                        .AsNoTracking()
                        .ById(request.UserId)
                        .SelectMany(x => x.Communications)
                        .WithScope(request.Scope)
                        .WithFilter(_filters, request)
                        .ProjectTo<CommunicationDto>(_mapper.ConfigurationProvider)
                        .PaginateAsync(request, cancellationToken);
}