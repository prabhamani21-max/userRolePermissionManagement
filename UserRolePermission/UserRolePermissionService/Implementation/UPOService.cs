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

        public UPOService(IUPORepository repository, IMapper mapper, IRolePermissionService rolePermissionService)
        {
            _repository = repository;
            _mapper = mapper;
            _rolePermissionService = rolePermissionService;
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
    }
}
