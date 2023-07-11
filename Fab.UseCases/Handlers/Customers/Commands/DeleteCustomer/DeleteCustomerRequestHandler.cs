using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Customers.Commands.DeleteCustomer;

public class DeleteCustomerRequestHandler : IRequestHandler<DeleteCustomerRequest>
{
    private readonly IDbContext _dbContext;

    public DeleteCustomerRequestHandler(IDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task<Unit> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
                           .WithScope(request.Scope)
                           .ById(request.CustomerId)
                           .FirstOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException("Заказчик не найден");

        _dbContext.Remove(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}