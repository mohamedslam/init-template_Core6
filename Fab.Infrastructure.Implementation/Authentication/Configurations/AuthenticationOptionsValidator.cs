using Fab.Infrastructure.Interfaces.Authentication;
using Microsoft.Extensions.Options;
using System.Text;

namespace Fab.Infrastructure.Implementation.Authentication.Configurations;

public class AuthenticationOptionsValidator : IValidateOptions<AuthenticationOptions>
{
    public ValidateOptionsResult Validate(string name, AuthenticationOptions options)
    {
        if (Encoding.UTF8.GetByteCount(options.SigningKey ?? "") < 16)
        {
            return ValidateOptionsResult.Fail(
                $"{nameof(AuthenticationOptions.SigningKey)} must be greater than 128 bits");
        }

        return ValidateOptionsResult.Success;
    }
}