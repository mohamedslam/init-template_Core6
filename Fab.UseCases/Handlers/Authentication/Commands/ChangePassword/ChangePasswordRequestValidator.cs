using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab.UseCases.Handlers.Authentication.Commands.ChangePassword
{
    public class ChangePasswordRequestValidator:AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
