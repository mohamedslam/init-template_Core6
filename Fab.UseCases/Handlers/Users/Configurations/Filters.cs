using AltPoint.Filters.Definitions;
using Fab.Entities.Models.Communications;
using Fab.Entities.Models.Users;

namespace Fab.UseCases.Handlers.Users.Configurations;

public class Filters : IFilterDefinition
{
    public void Register(IFilterCollectionBuilder builder)
    {
        builder.ForEntity<User>()
            .Property(x => x.Id)
            .Property(x => x.Name)
            .Property(x => x.Surname)
            .Property(x => x.Patronymic)           
            .Property(x => x.IsBlocked, "Blocked")
            .Property(x => x.Role)
            .Relation(x => x.Tenants)
            .Relation(x => x.TenantsUsers)
            .Ignore(x => x.Tokens)
            .Ignore(x=>x.Password)
            .Relation(x => x.Communications)
            .Property(x => x.LastLoginAt)
            .Property(x => x.CreatedAt)
            .Property(x => x.UpdatedAt);

        builder.ForEntity<Communication>()
            .Property(x => x.Id)
            .Property(x => x.Type)
            .Property(x => x.Value)
            .Property(x => x.Confirmed)
            .Property(x => x.DeviceId)
            .Relation(x => x.User).Ignore(x => x.UserId)
            .Ignore(x => x.Verifications)
            .Property(x => x.CreatedAt)
            .Property(x => x.UpdatedAt);
    }
}