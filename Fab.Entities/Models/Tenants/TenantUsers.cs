using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Tenants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab.Entities.Models.Tenants;

[Table("TenantsUsers")]
public class TenantUsers : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public TenantsRole Role{ get; set; }     
    public DateTime CreatedAt { get; set ; }
    public DateTime UpdatedAt { get ; set ; }
}
