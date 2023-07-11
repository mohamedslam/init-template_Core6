using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Users.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Queries.ListUsers;

public class ListUsersRequestHandler : IRequestHandler<ListUsersRequest, Page<UserDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListUsersRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<UserDto>> Handle(ListUsersRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Users
                        .AsTracking()
                        .AsSplitQuery()
                        .WithScope(request.Scope)
                        .WithFilter(_filters, request)
                        .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                        .PaginateAsync(request, cancellationToken);
}