using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions;
using Fab.Entities.Models.Tenants;
using Fab.UseCases.Support.Scopes;
using MediatR;

namespace Fab.UseCases.Handlers.Tenants.Commands.DeleteTenant;

public class DeleteTenantRequest : IRequest, IScopedRequest<Tenant>
{
    public Spec<Tenant>? Scope { get; set; }
    
    [NotMapped]
    public Guid TenantId { get; set; }
}