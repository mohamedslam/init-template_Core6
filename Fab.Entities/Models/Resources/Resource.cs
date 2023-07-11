using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Models.Users;

namespace Fab.Entities.Models.Resources;

[Table("Resources")]
public class Resource : IEntity, IHasTimestamps, ISoftDeletes
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string Bucket { get; set; } = null!;
    public string Target { get; set; } = null!;

    public Guid CreatorId { get; set; }
    public User Creator { get; set; } = null!;

    public string OriginalName { get; set; } = null!;
    public long Size { get; set; }
    public string ContentType { get; set; } = null!;
    public string Extension { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>(); // avatars

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}