
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Service.Interface
{
    public interface IRoleService
    {
        Task<int> CreateRoleAsync(Role role);
        Task<List<Role>> GetAllRoles();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> UpdateRoleAsync(Role role);
        Task<(bool Success, string Message)> DeleteRoleAsync(int id);
    }
}
