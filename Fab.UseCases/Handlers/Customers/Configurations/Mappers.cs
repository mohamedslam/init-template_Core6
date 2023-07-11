using AutoMapper;
using Fab.Entities.Models.Customers;
using Fab.UseCases.Handlers.Customers.Commands.CreateCustomer;
using Fab.UseCases.Handlers.Customers.Commands.UpdateCustomer;
using Fab.UseCases.Handlers.Customers.Dto;
using Fab.UseCases.Support;

namespace Fab.UseCases.Handlers.Customers.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<Customer, CustomerDto>();

        CreateMap<CreateCustomerRequest, Customer>()
            .IgnoreDefaults()
            .Ignore(dest => dest.Projects);

        CreateMap<UpdateCustomerRequest, Customer>()
            .IgnoreDefaults()
            .Ignore(dest => dest.Projects);
    }
}