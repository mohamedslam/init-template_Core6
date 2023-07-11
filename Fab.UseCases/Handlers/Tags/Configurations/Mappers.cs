using AutoMapper;
using Fab.Entities.Models.Tags;
using Fab.UseCases.Handlers.Tags.Commands.CreateTag;
using Fab.UseCases.Handlers.Tags.Commands.UpdateTag;
using Fab.UseCases.Handlers.Tags.Dto;
using Fab.UseCases.Support;

namespace Fab.UseCases.Handlers.Tags.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<Tag, TagDto>();

        CreateMap<CreateTagRequest, Tag>()
            .IgnoreDefaults();
        
        CreateMap<UpdateTagRequest, Tag>()
            .IgnoreDefaults();
    }
}