using AutoMapper;
using Fab.ApplicationServices.Interfaces.Communications;
using Fab.Entities.Enums.Communications;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Exceptions;
using Fab.Utils.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fab.UseCases.Handlers.Authentication.Commands.ChangePassword
{
    public class ChangePasswordRequestHandler : IRequestHandler<ChangePasswordRequest>
    {
        private readonly IDbContext _dbContext;
        private readonly ICommunicationService _communicationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEncryptionAndDecryptionService _encryptionAndDecryptionService;
        private readonly IMapper _mapper;

        public ChangePasswordRequestHandler(IDbContext dbContext,
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

        public async Task<Unit> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                                  .Where(u => u.Communications
                                               .Any(x => x.Id == request.CommunicationId))
                                  .Include(x => x.Communications).ThenInclude(x => x.Verifications)
                                  .FirstOrDefaultAsync(cancellationToken)
                  ?? throw new NotFoundException("Коммуникация не найдена");

            var communication = user.Communications
                                    .First(x => x.Id == request.CommunicationId);

            var verifications = communication.Verifications.FirstOrDefault();

            if (request.Password != _encryptionAndDecryptionService.Decrypt(user.Password!, "Alt.Point"))
                throw new ForbiddenException("Неверный пароль");
            else if (request.Code != verifications.Code)
                throw new ForbiddenException("Неверный Код");
            //ToDo Change Period for Expired Code
            else if (DateTime.UtcNow > verifications.CreatedAt.AddMinutes(5))
                throw new ForbiddenException("Неверный Код");
            else if (_encryptionAndDecryptionService.Decrypt(user.Password, "Alt.Point") != request.Password)
                throw new ForbiddenException("Недействительный старый код доступа");

            user.Password= _encryptionAndDecryptionService.Encrypt(request.NewPassword, "Alt.Point");
                
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);    

            return Unit.Value;
        }
    }
}
