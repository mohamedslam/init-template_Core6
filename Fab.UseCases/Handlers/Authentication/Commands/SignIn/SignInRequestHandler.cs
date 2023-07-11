using AutoMapper;
using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Exceptions;
using Fab.UseCases.Handlers.Authentication.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignIn;

public class SignInRequestHandler : IRequestHandler<SignInRequest, AuthTokenDto>
{
    private readonly IDbContext _dbContext;
    private readonly ICommunicationService _communicationService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly IEncryptionAndDecryptionService _encryptionAndDecryptionService;

    public SignInRequestHandler(IDbContext dbContext,
                                ICommunicationService communicationService,
                                IAuthenticationService authenticationService, 
                                IEncryptionAndDecryptionService encryptionAndDecryptionService,
                                IMapper mapper)
    {
        _dbContext = dbContext;
        _communicationService = communicationService;
        _authenticationService = authenticationService;
        _encryptionAndDecryptionService = encryptionAndDecryptionService;
        _mapper = mapper;
    }

    public async Task<AuthTokenDto> Handle(SignInRequest request, CancellationToken cancellationToken)
    {

        var communication = await _dbContext.Communications
                                            .Where(Communication.ByTypeAndValue(
                                                request.Communication.Type,
                                                request.Communication.Value))
                                            .Include(x => x.User).ThenInclude(x => x.Tokens)
                                            .Include(x => x.Verifications)
                                            .Where(x => x.UserId.HasValue &&
                                                        !x.User.IsBlocked)
                                            .OrderByDescending(x => x.Confirmed)
                                            .FirstOrDefaultAsync(cancellationToken)
                            ?? throw new NotFoundException("Коммуникация не найдена");

        if (!communication.Confirmed)
            throw new ForbiddenException("Выбранный контакт еще не подтвержден");
        
        if(communication.User.IsBlocked)
            throw new ForbiddenException("Пользователь заблокирован");

        //ToDo Decraption password key  
        if (request.Password != _encryptionAndDecryptionService.Decrypt(communication.User.Password!, "Alt.Point"))
            throw new ForbiddenException("Неверный логин или пароль");

        communication.User.LastLoginAt = DateTime.UtcNow;

        var auth = _authenticationService.GenerateToken(communication.User, request);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AuthTokenDto>(auth);
    }
}