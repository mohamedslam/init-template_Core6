using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Fab.ApplicationServices.Interfaces;
using Fab.Entities.Models.Resources;
using Fab.UseCases.Handlers.Resources.Commands.CreateResource;
using Fab.UseCases.Handlers.Resources.Commands.UpdateResource;
using Fab.UseCases.Handlers.Resources.Dto;
using Fab.UseCases.Support;
using Fab.Utils.Extensions;

namespace Fab.UseCases.Handlers.Resources.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<Guid, Resource>()
            .EqualityComparison((src, dest) => dest.Id == src)
            .ConvertUsing(src => new Resource { Id = src });

        CreateMap<Resource, ResourceDto>()
            .Map(dest => dest.MimeType, src => src.ContentType);

        CreateMap<CreateResourceRequest, Resource>()
            .IgnoreDefaults()
            .Ignore(dest => dest.Bucket)
            .Ignore(dest => dest.Creator)
            .Ignore(dest => dest.Users)
            .Map(dest => dest.CreatorId, (_, _, ctx) => ctx.Items[nameof(IContext)]
                                                           .As<IContext>()
                                                           .UserId)
            .Map(dest => dest.Target, (_, _, ctx) => ctx.Items[nameof(Resource.Target)])
            .Map(dest => dest.Size, src => src.File.Size)
            .Map(dest => dest.Extension, src => src.File.Extension)
            .Map(dest => dest.ContentType, src => src.File.ContentType)
            .Map(dest => dest.OriginalName, src => src.File.OriginalName);

        CreateMap<UpdateResourceRequest, Resource>(MemberList.Source)
            .IgnoreDefaults()
            .ForSourceMember(src => src.ResourceId, opt => opt.DoNotValidate());
    }
}