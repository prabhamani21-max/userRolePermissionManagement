using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;

namespace UserRolePermission.Repository.Interface
{
    public interface IRoleRepository
    {
        Task<int> CreateRoleAsync(Role role);
        Task<List<Role>> GetAllRoles();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> UpdateRoleAsync(Role role);
        Task<(bool Success, string Message)> DeleteRoleAsync(int id);
    }
}
