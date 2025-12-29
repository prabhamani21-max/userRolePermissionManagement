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

        public UPOService(IUPORepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<long> CreateUserPermissionOverrideAsync(UserPermissionOverride userPermissionOverride)
        {
            return await _repository.CreateUserPermissionOverrideAsync(userPermissionOverride);
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
            return await _repository.UpdateUserPermissionOverrideAsync(userPermissionOverride);
        }

        public async Task<(bool Success, string Message)> DeleteUserPermissionOverrideAsync(long id)
        {
            return await _repository.DeleteUserPermissionOverrideAsync(id);
        }
    }
}
