using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Customers.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Customers.Queries.ListCustomers;

public class ListCustomersRequestHandler : IRequestHandler<ListCustomersRequest, Page<CustomerDto>>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListCustomersRequestHandler(IReadonlyDbContext dbContext, IFiltersCollection filters, IMapper mapper)
    {
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<CustomerDto>> Handle(ListCustomersRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Customers
                        .AsNoTracking()
                        .WithScope(request.Scope)
                        .WithFilter(_filters, request)
                        .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                        .PaginateAsync(request, cancellationToken);
}