using AutoMapper;
using Fab.Entities.Models.Users;
using Fab.UseCases.Handlers.Users.Commands.UpdateUser;
using Fab.UseCases.Handlers.Users.Dto;
using Fab.UseCases.Support;

namespace Fab.UseCases.Handlers.Users.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<User, UserDto>()
            .Map(dest => dest.Blocked, src => src.IsBlocked);

        CreateMap<UpdateUserRequest, User>()
            .IgnoreDefaults()
            .Ignore(dest => dest.Role)
            .Ignore(dest => dest.IsBlocked)
            .Ignore(dest => dest.Tokens)
            .Ignore(dest => dest.Communications)
            .Ignore(dest => dest.LastLoginAt)
            .Ignore(dest => dest.Tenants)
            .Ignore(dest => dest.TenantsUsers)
            .Ignore(dest => dest.Password);
    }
}