using AltPoint.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.ApplicationServices.Interfaces;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Handlers.Notifications.Dto;
using Fab.UseCases.Support;
using Fab.UseCases.Support.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Notifications.Queries.ListNotifications;

public class ListNotificationsRequestHandler : IRequestHandler<ListNotificationsRequest, Page<NotificationDto>>
{
    private readonly IContext _context;
    private readonly IReadonlyDbContext _dbContext;
    private readonly IFiltersCollection _filters;
    private readonly IMapper _mapper;

    public ListNotificationsRequestHandler(IContext context, IReadonlyDbContext dbContext, IFiltersCollection filters,
                                           IMapper mapper)
    {
        _context = context;
        _dbContext = dbContext;
        _filters = filters;
        _mapper = mapper;
    }

    public async Task<Page<NotificationDto>> Handle(ListNotificationsRequest request,
                                                    CancellationToken cancellationToken) =>
        await _dbContext.Notifications
                        .AsNoTracking()
                        .Where(x => x.ReceiverId == _context.UserId)
                        .WithFilter(_filters, request)
                        .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                        .PaginateAsync(request, cancellationToken);
}