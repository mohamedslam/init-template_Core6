using Fab.Entities.Abstractions;
using Fab.Entities.Models.Tenants;
using Fab.UseCases.Handlers.Tenants.Dto;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Tenants.Queries.ReadTenant;

public class ReadTenantRequest : IRequest<TenantDto>, IScopedRequest<Tenant>
{
    [NotMapped]
    public Spec<Tenant>? Scope { get; set; }

    [NotMapped]
    public Guid TenantId { get; set; }
}