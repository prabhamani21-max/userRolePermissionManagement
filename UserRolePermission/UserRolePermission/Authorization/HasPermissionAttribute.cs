using Microsoft.AspNetCore.Authorization;

namespace UserRolePermission.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
        {
            Policy = $"HasPermission:{permission}";
        }
    }
}