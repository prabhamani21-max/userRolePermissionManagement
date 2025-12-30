using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Service.Interface
{
    public interface IRolePermissionService
    {
        Task<int> CreateRolePermissionAsync(RolePermission rolePermission);
        Task<Pagination<RolePermission>> GetAllRolePermissionsAsync(int? roleId, int? actionId, int? statusId, int pageNumber = 1, int pageSize = 10);
        Task<RolePermission> GetRolePermissionByIdAsync(int id);
        Task<RolePermission> UpdateRolePermissionAsync(RolePermission rolePermission);
        Task<(bool Success, string Message)> DeleteRolePermissionAsync(int id);
        Task<List<string>> GetEffectivePermissionsAsync(long userId);
        Task InvalidatePermissionsCacheAsync();
    }
}
