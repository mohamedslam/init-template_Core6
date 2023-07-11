using Fab.Infrastructure.DataAccess.Interfaces;
using FluentValidation;

namespace Fab.UseCases.Handlers.Customers.Commands.CreateCustomer;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator(IDbContext dbContext)
    {
        
    }
}