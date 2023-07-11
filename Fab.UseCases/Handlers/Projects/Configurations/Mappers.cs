using AutoMapper;
using Fab.Entities.Models.Projects;
using Fab.UseCases.Handlers.Projects.Commands.CreateProject;
using Fab.UseCases.Handlers.Projects.Commands.UpdateProject;
using Fab.UseCases.Handlers.Projects.Dto;
using Fab.UseCases.Support;

namespace Fab.UseCases.Handlers.Projects.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<Project, ProjectDto>();

        CreateMap<CreateProjectRequest, Project>()
            .IgnoreDefaults()
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.ModelIFC);

        CreateMap<UpdateProjectRequest, Project>()
            .IgnoreDefaults()
            .Ignore(dest => dest.Customer)
            .Ignore(dest => dest.ModelIFC);
    }
}