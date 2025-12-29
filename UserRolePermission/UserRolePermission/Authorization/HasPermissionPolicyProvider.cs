using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace UserRolePermission.Authorization
{
    public class HasPermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public HasPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("HasPermission:", StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName.Substring("HasPermission:".Length);
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new HasPermissionRequirement(permission))
                    .Build();
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}