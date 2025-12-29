using Microsoft.AspNetCore.Authorization;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Authorization
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public HasPermissionHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var rolePermissionService = scope.ServiceProvider.GetRequiredService<IRolePermissionService>();

            var effectivePermissions = await rolePermissionService.GetEffectivePermissionsAsync(userId);

            if (effectivePermissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}