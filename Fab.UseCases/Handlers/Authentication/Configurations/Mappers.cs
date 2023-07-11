using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Fab.Entities.Models.Communications;
using Fab.Entities.Models.Users;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Dto;
using Fab.UseCases.Handlers.Authentication.Commands.SignUp;
using Fab.UseCases.Handlers.Authentication.Dto;
using Fab.UseCases.Support;
using Profile = AutoMapper.Profile;

namespace Fab.UseCases.Handlers.Authentication.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<Communication, CommunicationDto>();
        CreateMap<CommunicationDto, Communication>()
            .EqualityComparison((dto, comm) => dto.Id == comm.Id)
            .Ignore(dest => dest.User)
            .Ignore(dest => dest.UserId!)
            .Ignore(dest => dest.Verifications);

        CreateMap<Communication, CommunicationShortDto>();
        CreateMap<CommunicationShortDto, Communication>()
            .EqualityComparison((dto, comm) => dto.Type == comm.Type &&
                                               dto.Value == comm.Value)
            .IgnoreDefaults()
            .Ignore(dest => dest.User)
            .Ignore(dest => dest.UserId)
            .Ignore(dest => dest.Verifications)
            .Ignore(dest => dest.Confirmed);

        CreateMap<CommunicationShort, CommunicationShortDto>();
        CreateMap<CommunicationShortDto, CommunicationShort>()
            .ConvertUsing(src => new CommunicationShort(src.Type, src.Value));

        CreateMap<AuthToken, AuthTokenDto>();

        CreateMap<SignUpRequest, User>(MemberList.Source);
    }
}