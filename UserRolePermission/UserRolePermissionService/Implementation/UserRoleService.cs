using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task<long> CreateUserRoleAsync(UserRole userRole)
        {
            return await _userRoleRepository.CreateUserRoleAsync(userRole);
        }

        public async Task<Pagination<UserRole>> GetAllUserRolesAsync(int? statusId, long? userId, int? roleId, int pageNumber = 1, int pageSize = 10)
        {
            return await _userRoleRepository.GetAllUserRolesAsync(statusId, userId, roleId, pageNumber, pageSize);
        }

        public async Task<UserRole> GetUserRoleByIdAsync(long id)
        {
            return await _userRoleRepository.GetUserRoleByIdAsync(id);
        }

        public async Task<UserRole> UpdateUserRoleAsync(UserRole userRole)
        {
            return await _userRoleRepository.UpdateUserRoleAsync(userRole);
        }

        public async Task<(bool Success, string Message)> DeleteUserRoleAsync(long id)
        {
            return await _userRoleRepository.DeleteUserRoleAsync(id);
        }

        public async Task<List<UserRole>> GetUserRolesByUserIdAsync(long userId)
        {
            return await _userRoleRepository.GetUserRolesByUserIdAsync(userId);
        }
    }
}
