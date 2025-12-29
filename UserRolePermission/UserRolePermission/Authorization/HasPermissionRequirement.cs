using Microsoft.AspNetCore.Authorization;

namespace UserRolePermission.Authorization
{
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public HasPermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}