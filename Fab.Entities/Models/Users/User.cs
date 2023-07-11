using System.ComponentModel.DataAnnotations.Schema;
using Fab.Entities.Abstractions.Attributes;
using Fab.Entities.Abstractions.Interfaces;
using Fab.Entities.Enums.Users;
using Fab.Entities.Models.Communications;
using Fab.Entities.Models.Tenants;
using Fab.Utils.Extensions;

namespace Fab.Entities.Models.Users;

[Table("Users")]
public class User : IEntity, IHasTimestamps
{
    public Guid Id { get; set; }

    [Searchable]
    public string? Name { get; set; }

    [Searchable]
    public string? Surname { get; set; } 

    [Searchable]
    public string? Patronymic { get; set; }

    public string? Password { get; set; }
    public Role Role { get; set; } = Role.User;    
    public bool IsBlocked { get; set; }
    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
    public virtual ICollection<Communication> Communications { get; set; } = new List<Communication>();
    public virtual ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
    public virtual ICollection<TenantUsers> TenantsUsers { get; set; } = new List<TenantUsers>();
    public DateTime LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [NotMapped]
    public string? Fullname =>
        JoinFullname(Surname, Name, Patronymic);

    public static string? JoinFullname(params string?[] parts) =>
        parts.Where(x => !string.IsNullOrWhiteSpace(x))
             .Join(" ")
             .Let(x => !string.IsNullOrWhiteSpace(x) ? x : null);

    public void CleanupExpiredTokens(TimeSpan interval) =>
        Tokens = Tokens.Where(t => !t.IsExpired(interval))
                       .ToList();
}