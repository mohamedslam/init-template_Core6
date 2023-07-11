using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Customers.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Customers.Queries.ReadCustomer;

public class ReadCustomerRequestHandler : IRequestHandler<ReadCustomerRequest, CustomerDto>
{
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadCustomerRequestHandler(IReadonlyDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(ReadCustomerRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Customers
                        .AsNoTracking()
                        .WithScope(request.Scope)
                        .ById(request.CustomerId)
                        .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException("Заказчик не найден");
}