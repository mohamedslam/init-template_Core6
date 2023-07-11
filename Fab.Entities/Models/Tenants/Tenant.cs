using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions.Attributes;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Tenants;

namespace Fab.Entities.Models.Tenants;

[Table("Tenants")]
public class Tenant : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    [Searchable]
    public string Label { get; set; } = null!;   
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}