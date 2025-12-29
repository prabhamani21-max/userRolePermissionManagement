using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IMemoryCache _cache;
        private static int _cacheVersion = 0;

        public RolePermissionService(IRolePermissionRepository rolePermissionRepository, IMemoryCache cache)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _cache = cache;
        }

        public async Task<int> CreateRolePermissionAsync(RolePermission rolePermission)
        {
            var result = await _rolePermissionRepository.CreateRolePermissionAsync(rolePermission);
            Interlocked.Increment(ref _cacheVersion);
            return result;
        }

        public async Task<Pagination<RolePermission>> GetAllRolePermissionsAsync(int? roleId, int? actionId, int? statusId, int pageNumber = 1, int pageSize = 10)
        {
            return await _rolePermissionRepository.GetAllRolePermissionsAsync(roleId, actionId, statusId, pageNumber, pageSize);
        }

        public async Task<RolePermission> GetRolePermissionByIdAsync(int id)
        {
            return await _rolePermissionRepository.GetRolePermissionByIdAsync(id);
        }

        public async Task<RolePermission> UpdateRolePermissionAsync(RolePermission rolePermission)
        {
            var result = await _rolePermissionRepository.UpdateRolePermissionAsync(rolePermission);
            Interlocked.Increment(ref _cacheVersion);
            return result;
        }

        public async Task<(bool Success, string Message)> DeleteRolePermissionAsync(int id)
        {
            var result = await _rolePermissionRepository.DeleteRolePermissionAsync(id);
            Interlocked.Increment(ref _cacheVersion);
            return result;
        }

        public async Task<List<string>> GetEffectivePermissionsAsync(long userId)
        {
            var cacheKey = $"Permissions_{userId}_{_cacheVersion}";

            if (_cache.TryGetValue(cacheKey, out List<string> permissions))
            {
                return permissions;
            }

         permissions = await _rolePermissionRepository.GetEffectivePermissionsAsync(userId);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(7));

            _cache.Set(cacheKey, permissions, cacheEntryOptions);

            return permissions;
        }
    }
}
