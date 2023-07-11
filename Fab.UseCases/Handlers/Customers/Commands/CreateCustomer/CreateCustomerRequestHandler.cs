using AutoMapper;
using Fab.Entities.Models.Customers;
using Fab.Infrastructure.DataAccess.Interfaces;
using MediatR;

namespace Fab.UseCases.Handlers.Customers.Commands.CreateCustomer;

public class CreateCustomerRequestHandler : IRequestHandler<CreateCustomerRequest, Guid>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public CreateCustomerRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = _mapper.Map<Customer>(request);

        _dbContext.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}