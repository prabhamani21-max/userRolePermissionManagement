using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Repository.Implementation;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> CreateRoleAsync(Role role)
        {
            return await _repository.CreateRoleAsync(role);
        }
        public async Task<List<Role>> GetAllRoles()
        {
            return await _repository.GetAllRoles();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _repository.GetRoleByIdAsync(id);
        }
        public async Task<Role> UpdateRoleAsync(Role role)
        {
            return await _repository.UpdateRoleAsync(role);
        }

        public async Task<(bool Success, string Message)> DeleteRoleAsync(int id)
        {
            return await _repository.DeleteRoleAsync(id);
        }
    }
}
