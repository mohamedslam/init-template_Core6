using Fab.Entities.Abstractions.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.Entities.Models.Communications;

[Table("CommunicationVerifications")]
public class Verification : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    public Guid CommunicationId { get; set; }
    public Communication Communication { get; set; } = null!;

    public string Code { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}