using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions.Attributes;
using Fab.Entities.Abstractions.Interfaces;

namespace Fab.Entities.Models.Projects;

[Table("ModelIFC")]
public class ProjectModel : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }
    [Searchable]
    public string Label { get; set; } = null!;
    [Searchable]
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}