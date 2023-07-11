using AutoMapper;
using Fab.Entities.Models.Notifications;
using Fab.UseCases.Handlers.Notifications.Dto;

namespace Fab.UseCases.Handlers.Notifications.Configurations;

public class Mappers : Profile
{
    public Mappers()
    {
        CreateMap<Notification, NotificationDto>();
    }
}