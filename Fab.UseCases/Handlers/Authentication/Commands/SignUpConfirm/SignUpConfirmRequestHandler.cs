using AutoMapper;
using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignUpConfirm;

public class SignUpConfirmRequestHandler : IRequestHandler<SignUpConfirmRequest>
{
    private readonly IDbContext _dbContext;
    private readonly ICommunicationService _communicationService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEncryptionAndDecryptionService _encryptionAndDecryptionService;
    private readonly IMapper _mapper;


    public SignUpConfirmRequestHandler(IDbContext dbContext,
                                ICommunicationService communicationService,
                                IEncryptionAndDecryptionService encryptionAndDecryptionService,
                                IMapper mapper)
    {
        _dbContext = dbContext;
        _communicationService = communicationService;
        _encryptionAndDecryptionService = encryptionAndDecryptionService;
        _mapper = mapper;
    }
    public async Task<Unit> Handle(SignUpConfirmRequest request, CancellationToken cancellationToken)
    {

        var communication = _dbContext.Communications
                                     .Include(v => v.Verifications)
                                     .First(x => x.Id == request.CommunicationId );

        //ToDo Specification Period of Expired  SMS Code Verification from appsetting.json
        var verifications = communication.Verifications
                              .First();

        if(communication.Confirmed==true)
            throw new ForbiddenException("Способ связи уже подтвержден");
        
        if (verifications.Code != request.Code)
           throw new ForbiddenException("Указанный код неверный");
        else if (verifications.CreatedAt.AddMinutes(5) <= DateTime.UtcNow)
            throw new ForbiddenException("Срок действия кода истек");
        else if (verifications.Code != request.Code)
            throw new ForbiddenException("Указанный код неверный");

        communication.Confirmed = true;
        _dbContext.Update(communication);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
