using Fab.Entities.Abstractions.Attributes;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Models.Users;

namespace Fab.Entities.Models.Notifications;

public class Notification : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    public Guid ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;

    public string? EntityType { get; set; }
    public string? EntityId { get; set; }

    [Searchable]
    public string? Title { get; set; }

    [Searchable]
    public string Description { get; set; } = null!;

    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}