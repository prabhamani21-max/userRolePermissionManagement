using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Service.Interface
{
    public interface IUserService
    {
        Task<long> RegisterUserAsync(User user);
        Task<Pagination<User>> GetAllUsersAsync( int? statusId, string? name, int pageNumber = 1,
    int pageSize = 10);
        Task<User> GetUserByIdAsync(long id);
        Task<User> UpdateUserAsync(User user);
        Task<(bool Success, string Message)> DeleteUserAsync(long id);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<int>> GetUserRoleIdsAsync(long userId);


    }
}
