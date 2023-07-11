using Fab.Infrastructure.DataAccess.Interfaces;
using FluentValidation;

namespace Fab.UseCases.Handlers.Tenants.Commands.CreateTenant;

public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator(IDbContext dbContext)
    {
        
    }
}