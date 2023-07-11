using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions.Attributes;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Models.Projects;

namespace Fab.Entities.Models.Customers;

[Table("Customers")]
public class Customer : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }
    [Searchable]
    public string Label { get; set; } = null!;
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}