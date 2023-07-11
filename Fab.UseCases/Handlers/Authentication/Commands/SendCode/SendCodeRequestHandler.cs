using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Authentication.Commands.SendCode;

public class SendCodeHandler : IRequestHandler<SendCodeRequest, Guid>
{
    private readonly IDbContext _dbContext;
    private readonly ICommunicationService _communicationService;

    public SendCodeHandler(IDbContext dbContext, ICommunicationService communicationService)
    {
        _dbContext = dbContext;
        _communicationService = communicationService;
    }

    public async Task<Guid> Handle(SendCodeRequest request, CancellationToken cancellationToken)
    {
        var communication = await _dbContext.Communications
                                            .Where(Communication.ByTypeAndValue(
                                                request.Communication.Type,
                                                request.Communication.Value))
                                            .Include(x => x.User).ThenInclude(x => x.Tokens)
                                            .Include(x => x.Verifications)
                                            .Where(x => x.UserId.HasValue &&
                                                        !x.User.IsBlocked) // TODO Переданная коммуникация заблокированного пользователя удаляется при входе
                                            .OrderByDescending(x => x.Confirmed)
                                            .FirstOrDefaultAsync(cancellationToken)
                            ?? throw new NotFoundException("Коммуникация не найдена");

        await _communicationService.SendVerifyCodeAsync(communication, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return communication.Id;
    }
}