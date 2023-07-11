using MediatR;

namespace Fab.UseCases.Handlers.Tenants.Commands.CreateTenant;

public class CreateTenantRequest : IRequest<Guid>
{
    /// <summary>
    ///     Название тенанта
    /// </summary>
    public string Label { get; set; } = null!;
    public Guid UserId { get; set; }    
}