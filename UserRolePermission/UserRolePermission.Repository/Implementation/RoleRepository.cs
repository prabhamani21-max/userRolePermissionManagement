using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Repository.Data;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Repository.Models;

namespace UserRolePermission.Repository.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public RoleRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> CreateRoleAsync(Role role)
        {
            var entity = _mapper.Map<RoleDb>(role);
            await _context.Roles.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task<List<Role>> GetAllRoles()
        {
            return await _context.Roles
                .Where(r => r.Status == r.Status)
                .OrderBy(r => r.Id)
                .Select(r => new Role
                {
                    Id = r.Id,
                    Name = r.Name,
                })
                .ToListAsync();
        }
        public async Task<Role> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            return _mapper.Map<Role>(role);
        }
        public async Task<Role> UpdateRoleAsync(Role role)
        {
            var existingRole = await _context.Roles.FindAsync(role.Id);
            if (existingRole == null) return null;
            existingRole.Name = role.Name;
            existingRole.StatusId = role.StatusId;
            existingRole.UpdatedDate = DateTime.UtcNow;
            existingRole.UpdatedBy = role.UpdatedBy;

            _context.Roles.Update(existingRole);
            await _context.SaveChangesAsync();
            return new Role
            {
                Id = role.Id,
                Name = role.Name,
                StatusId = role.StatusId,
                UpdatedDate = role.UpdatedDate,
                UpdatedBy = role.UpdatedBy,
            };
        }
        public async Task<(bool Success, string Message)> DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return (false, "Role not found");

            role.StatusId = 3; // Deleted
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return (true, "Role deleted successfully");
        }
    }
}
