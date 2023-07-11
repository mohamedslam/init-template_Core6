using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fab.Entities.Models.Communications;

[Table("Communications")]
public partial class Communication : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    public CommunicationType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? DeviceId { get; set; }
    public bool Confirmed { get; set; }

    public Guid? UserId { get; set; }
    public User User { get; set; } = null!;
    public virtual ICollection<Verification> Verifications { get; set; } = new List<Verification>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}