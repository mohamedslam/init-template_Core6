using AutoMapper;
using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Users;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.Utils.Extensions;
using MediatR;

namespace Fab.UseCases.Handlers.Authentication.Commands.SignUp;

public class SignUpRequestHandler : IRequestHandler<SignUpRequest, Guid>
{
    private readonly IDbContext _dbContext;
    private readonly ICommunicationService _communicationService;
    private readonly IMapper _mapper;
    private readonly IEncryptionAndDecryptionService _encryptionAndDecryptionService;

    public SignUpRequestHandler(IDbContext dbContext,
                                ICommunicationService communicationService,
                                IEncryptionAndDecryptionService encryptionAndDecryptionService,
                                IMapper mapper)
    {
        _dbContext = dbContext;
        _communicationService = communicationService;
        _encryptionAndDecryptionService = encryptionAndDecryptionService;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(SignUpRequest request, CancellationToken cancellationToken)
    {
        //ToDo Encraption password
        request.Password = _encryptionAndDecryptionService.Encrypt(request.Password,"Alt.Point");

        var user = _mapper.Map<User>(request);
        _dbContext.Add(user);

        var primaryCommunication = user.Communications
                                       .SingleOrDefault(x => x.Type == CommunicationType.Phone)
                                   ?? user.Communications.First();

        await _communicationService.SendVerifyCodeAsync(
                                    primaryCommunication.Also(x => x.User = user),
                                    CancellationToken.None);
        
        await _dbContext.SaveChangesAsync(cancellationToken);


        return primaryCommunication.Id;
    }
}