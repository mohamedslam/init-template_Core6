using AutoMapper;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Customers.Commands.UpdateCustomer;

public class UpdateCustomerRequestHandler : IRequestHandler<UpdateCustomerRequest>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateCustomerRequestHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
                           .WithScope(request.Scope)
                           .ById(request.CustomerId)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Заказчик не найден");

        _mapper.Map(request, customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}