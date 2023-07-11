using AutoMapper;
using AutoMapper.QueryableExtensions;
using Fab.ApplicationServices.Interfaces;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Notifications.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Notifications.Queries.ReadNotification;

public class ReadNotificationRequestHandler : IRequestHandler<ReadNotificationRequest, NotificationDto>
{
    private readonly IContext _context;
    private readonly IReadonlyDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadNotificationRequestHandler(IContext context, IReadonlyDbContext dbContext, IMapper mapper)
    {
        _context = context;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<NotificationDto> Handle(ReadNotificationRequest request, CancellationToken cancellationToken) =>
        await _dbContext.Notifications
                        .AsNoTracking()
                        .Where(x => x.ReceiverId == _context.UserId &&
                                    x.Id == request.NotificationId)
                        .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefaultAsync(cancellationToken)
        ?? throw new NotFoundException("Уведомление не найдено");
}