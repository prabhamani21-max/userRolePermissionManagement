using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Service.Interface
{
    public interface IUserRoleService
    {
        Task<long> CreateUserRoleAsync(UserRole userRole);
        Task<Pagination<UserRole>> GetAllUserRolesAsync(int? statusId, long? userId, int? roleId, int pageNumber = 1, int pageSize = 10);
        Task<UserRole> GetUserRoleByIdAsync(long id);
        Task<UserRole> UpdateUserRoleAsync(UserRole userRole);
        Task<(bool Success, string Message)> DeleteUserRoleAsync(long id);
        Task<List<UserRole>> GetUserRolesByUserIdAsync(long userId);
    }
}
