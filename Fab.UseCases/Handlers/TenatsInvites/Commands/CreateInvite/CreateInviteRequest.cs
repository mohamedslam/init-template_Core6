using Fab.Entities.Enums.Tenants;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab.UseCases.Handlers.TenatsInvites.Commands.CreateInvite
{
    public class CreateInviteRequest : IRequest<Guid>
    {
        public string Email { get; set; }
        public TenantsRole Role { get; set; }

        [NotMapped]
        public Guid TenantId { get; set; }

        [NotMapped]
        public Guid UserId { get; set; }

        public string Password { get; set; }
    }
}
