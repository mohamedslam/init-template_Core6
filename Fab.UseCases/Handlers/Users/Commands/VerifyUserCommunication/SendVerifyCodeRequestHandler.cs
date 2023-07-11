using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Specifications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Users.Commands.VerifyUserCommunication;

public class SendVerifyCodeRequestHandler : IRequestHandler<SendVerifyCodeRequest>
{
    private readonly IDbContext _dbContext;
    private readonly ICommunicationService _communicationService;

    public SendVerifyCodeRequestHandler(IDbContext dbContext, ICommunicationService communicationService)
    {
        _dbContext = dbContext;
        _communicationService = communicationService;
    }

    public async Task<Unit> Handle(SendVerifyCodeRequest request, CancellationToken cancellationToken)
    {
        var communication = await _dbContext.Communications
                                .ById(request.CommunicationId)
                                .Where(x => x.UserId == request.UserId)
                                .Include(x => x.User)
                                .Include(x => x.Verifications)
                                .FirstOrDefaultAsync(cancellationToken)
                            ?? throw new NotFoundException("Коммуникация не найдена");
        
        await _communicationService.SendVerifyCodeAsync(communication, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}