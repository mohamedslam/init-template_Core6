using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions.Interfaces;

namespace Fab.Entities.Models.Tags;

[Table("Tags")]
public class Tag : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    public string Label { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}