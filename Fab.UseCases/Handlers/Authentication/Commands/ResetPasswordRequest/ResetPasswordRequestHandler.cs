using AutoMapper;
using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Models.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Authentication.Commands.ResetPasswordRequest
{
    public class ResetPasswordRequestHandler : IRequestHandler<ResetPasswordRequest, Guid>
    {
        private readonly IDbContext _dbContext;
        private readonly ICommunicationService _communicationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEncryptionAndDecryptionService _encryptionAndDecryptionService;

        private readonly IMapper _mapper;

        public ResetPasswordRequestHandler(IDbContext dbContext,
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

        public async Task<Guid> Handle(Commands.ResetPasswordRequest.ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var communication = await _dbContext.Communications
                                    .Where(Communication.ByTypeAndValue(
                                        request.Communication.Type,
                                        request.Communication.Value))
                                    .Include(x => x.Verifications)
                                    .Where(x => x.UserId.HasValue &&
                                                !x.User.IsBlocked) 
                                    .OrderByDescending(x => x.Confirmed)
                                    .FirstOrDefaultAsync(cancellationToken)
                                ?? throw new NotFoundException("Коммуникация не найдена");

            await _communicationService.SendVerifyCodeAsync(communication, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return communication.Id;
        }
    }
}