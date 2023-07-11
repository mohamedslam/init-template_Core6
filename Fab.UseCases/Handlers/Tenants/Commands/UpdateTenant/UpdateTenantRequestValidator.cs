using FluentValidation;

namespace Fab.UseCases.Handlers.Tenants.Commands.UpdateTenant;

public class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequest>
{
    public UpdateTenantRequestValidator()
    {
        
    }
}