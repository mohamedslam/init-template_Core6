using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Fab.Entities.Abstractions.Interfaces;

namespace Fab.Entities.Models.Users;

[Table("Tokens")]
public class Token : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid Value { get; set; } = Guid.NewGuid();

    public string UserAgent { get; set; } = null!;
    public IPAddress IpAddress { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsExpired(TimeSpan interval) =>
        UpdatedAt != DateTime.MinValue &&
        UpdatedAt.Add(interval) < DateTime.UtcNow;

    public void Renew() =>
        Value = Guid.NewGuid();
}