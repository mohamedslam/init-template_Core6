using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions.Attributes;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Users;
using Fab.Entities.Models.Communications;
using Fab.Entities.Models.Customers;

namespace Fab.Entities.Models.Projects;

[Table("Projects")]
public class Project : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    [Searchable]
    public string Code { get; set; } = null!;

    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;
    
    [Searchable]
    public string Label { get; set; } = null!;
    [Searchable]
    public string Description { get; set; } = null!;
    public virtual ICollection<ProjectModel> ModelIFC { get; set; } = new List<ProjectModel>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}