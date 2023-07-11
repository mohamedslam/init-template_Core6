using Fab.Entities.Abstractions;
using Fab.Entities.Models.Tenants;
using Fab.UseCases.Support.Scopes;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.UseCases.Handlers.Tenants.Commands.UpdateTenant;

public class UpdateTenantRequest : IRequest, IScopedRequest<Tenant>
{
    [NotMapped]
    public Spec<Tenant>? Scope { get; set; }

    [NotMapped]
    public Guid TenantId { get; set; }

    /// <summary>
    ///     Название тенанта
    /// </summary>
    public string Label { get; set; } = null!;
}