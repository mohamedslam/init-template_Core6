using System.Reflection;
using Fab.Utils.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fab.Web.Policies.Support;

[Obsolete]
public class PolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly Dictionary<string, Type> _types;
    private readonly ILogger<PolicyProvider> _logger;

    public PolicyProvider(IOptions<AuthorizationOptions> options, ILogger<PolicyProvider> logger) : base(options)
    {
        _logger = logger;
        _types = new(
            Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => !x.IsGenericType &&
                                typeof(IAuthorizationRequirement).IsAssignableFrom(x))
                    .Select(x => KeyValuePair.Create(x.Name, x)));
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy != null)
        {
            return policy;
        }

        if (!_types.ContainsKey(policyName))
        {
            _logger.LogWarning("{PolicyName} not found", policyName);
            return null;
        }

        return new AuthorizationPolicyBuilder()
               .AddRequirements(Activator.CreateInstance(_types[policyName])!
                                         .As<IAuthorizationRequirement>())
               .Build();
    }
}