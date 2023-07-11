using AutoMapper;
using Fab.Entities.Models.Tenants;
using Fab.UseCases.Handlers.Tenants.Commands.CreateTenant;
using Fab.UseCases.Handlers.Tenants.Commands.UpdateTenant;
using Fab.UseCases.Handlers.Tenants.Dto;
using Fab.UseCases.Support;

namespace Fab.UseCases.Handlers.Tenants.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<Tenant, TenantDto>();

        CreateMap<CreateTenantRequest, Tenant>()
            .IgnoreDefaults();

        CreateMap<UpdateTenantRequest, Tenant>()
            .IgnoreDefaults();
    }
}