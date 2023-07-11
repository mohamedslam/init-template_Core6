using AutoMapper;
using Fab.Entities.Enums.Communications;
using Fab.Entities.Models.Communications;
using Fab.Entities.Models.Tenants;
using Fab.Entities.Models.Users;
using Fab.Infrastructure.DataAccess.Interfaces;
using Fab.Infrastructure.Interfaces.Authentication;
using Fab.UseCases.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab.UseCases.Handlers.TenatsInvites.Commands.CreateInvite
{
    public class CreateInviteRequestHandler : IRequestHandler<CreateInviteRequest, Guid>
    {
        private readonly IDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEncryptionAndDecryptionService _encryptionAndDecryptionService;

        public CreateInviteRequestHandler(IDbContext dbContext,
                                          IMapper mapper,
                                          IEncryptionAndDecryptionService encryptionAndDecryptionService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _encryptionAndDecryptionService = encryptionAndDecryptionService;
        }
        public async Task<Guid> Handle(CreateInviteRequest request, CancellationToken cancellationToken)
        {
            request.UserId = GetorCreateUserIdByEmail(request.Email,request.Password);   
    
            if (IsUserAlreadyInvited(request.TenantId,request.UserId))
                throw new NotFoundException("пользователь уже приглашен для этого Тенант");

            var tenantUserInvitationId = CreateInvitationUser(request);

            await _dbContext.SaveChangesAsync(cancellationToken);

            //ToDo Send that link Invitation at his mail 
            ////
            ////
            SendUserInvitation();
            return tenantUserInvitationId;
        }

        /// <summary>
        /// Создайте учетную запись пользователя, если она не найдена, и верните идентификатор
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private Guid GetorCreateUserIdByEmail(string email,string password) {
            Guid userId;
            var comunication = _dbContext.Communications.FirstOrDefault(x =>
                                                x.Value == email &&
                                                x.Type == CommunicationType.Email);

            if (comunication == null)
            {
                var user = new User()
                {
                    Password = _encryptionAndDecryptionService.Encrypt(password , "Alt.Point"),
                    Role = Entities.Enums.Users.Role.User,
                    CreatedAt = DateTime.UtcNow,
                };

                _dbContext.Add(user);
                userId = user.Id;
                var communication = new Communication()
                {
                    UserId = userId,
                    Type = CommunicationType.Email,
                    Value = email,
                    CreatedAt = DateTime.UtcNow,
                };
                _dbContext.Add(communication);
            }
            else
                userId= _dbContext.Users.First(x => x.Id == comunication.UserId).Id;

            return userId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Guid CreateInvitationUser(CreateInviteRequest request)
        {
            var tenantUser = new TenantUsers()
            {
                TenantId = request.TenantId,
                UserId = request.UserId,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Add(tenantUser);
            return tenantUser.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool IsUserAlreadyInvited(Guid tenantId,Guid userId)
        {
            var tenantUserInvite = _dbContext.TenantsUsers.FirstOrDefault(x => x.TenantId == tenantId  &&
                                                                      x.UserId == userId);
            if (tenantUserInvite != null)
                return true;
            else
                return false;
        }

        private void SendUserInvitation()
        {

        }
    }
}
