using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Repository.Interface
{
    public interface IUPORepository
    {
        Task<long> CreateUserPermissionOverrideAsync(UserPermissionOverride userPermissionOverride);
        Task<Pagination<UserPermissionOverride>> GetAllUserPermissionOverridesAsync(int? statusId, int pageNumber = 1, int pageSize = 10);
        Task<UserPermissionOverride> GetUserPermissionOverrideByIdAsync(long id);
        Task<UserPermissionOverride> UpdateUserPermissionOverrideAsync(UserPermissionOverride userPermissionOverride);
        Task<(bool Success, string Message)> DeleteUserPermissionOverrideAsync(long id);
    }
}
