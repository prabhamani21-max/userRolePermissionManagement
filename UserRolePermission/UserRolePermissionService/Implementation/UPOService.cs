using AutoMapper;
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
    public class UPOService : IUPOService
    {
        private readonly IUPORepository _repository;
        private readonly IMapper _mapper;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IUserService _userService;

        public UPOService(IUPORepository repository, IMapper mapper, IRolePermissionService rolePermissionService, IUserService userService)
        {
            _repository = repository;
            _mapper = mapper;
            _rolePermissionService = rolePermissionService;
            _userService = userService;
        }

        public async Task<long> CreateUserPermissionOverrideAsync(UserPermissionOverride userPermissionOverride)
        {
            var result = await _repository.CreateUserPermissionOverrideAsync(userPermissionOverride);
            await _rolePermissionService.InvalidatePermissionsCacheAsync();
            return result;
        }

        public async Task<Pagination<UserPermissionOverride>> GetAllUserPermissionOverridesAsync(int? statusId, int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetAllUserPermissionOverridesAsync(statusId, pageNumber, pageSize);
        }

        public async Task<UserPermissionOverride> GetUserPermissionOverrideByIdAsync(long id)
        {
            return await _repository.GetUserPermissionOverrideByIdAsync(id);
        }

        public async Task<UserPermissionOverride> UpdateUserPermissionOverrideAsync(UserPermissionOverride userPermissionOverride)
        {
            var result = await _repository.UpdateUserPermissionOverrideAsync(userPermissionOverride);
            await _rolePermissionService.InvalidatePermissionsCacheAsync();
            return result;
        }

        public async Task<(bool Success, string Message)> DeleteUserPermissionOverrideAsync(long id)
        {
            var result = await _repository.DeleteUserPermissionOverrideAsync(id);
            await _rolePermissionService.InvalidatePermissionsCacheAsync();
            return result;
        }

        public async Task<List<int>> GetUserEffectivePermissionsAsync(long userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return new List<int>();

            var rolePermissions = await _rolePermissionService.GetAllRolePermissionsAsync(user.DefaultRoleId, null, null, 1, int.MaxValue);
            var overrides = await _repository.GetAllUserPermissionOverridesAsync(null, 1, int.MaxValue);
            var userOverrides = overrides.Items.Where(o => o.UserId == userId).ToList();
            // What is ToHashSEt() here
            var roleActionIds = rolePermissions.Items.Select(rp => rp.ActionId).ToHashSet();
            var allowOverrides = userOverrides.Where(o => !o.IsDenied).Select(o => o.ActionId);
            var denyOverrides = userOverrides.Where(o => o.IsDenied).Select(o => o.ActionId);

            var effective = roleActionIds.Union(allowOverrides).Except(denyOverrides).ToList();
            return effective;
        }
    }
}
