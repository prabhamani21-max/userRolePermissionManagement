using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Repository.Data;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Repository.Models;

namespace UserRolePermission.Repository.Implementation
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly AppDBContext _context;

        public RolePermissionRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<int> CreateRolePermissionAsync(RolePermission rolePermission)
        {
            var rolePermissionDb = new RolePermissionDb
            {
                RoleId = rolePermission.RoleId,
                ActionId = rolePermission.ActionId,
                StatusId = rolePermission.StatusId,
                CreatedDate = rolePermission.CreatedDate,
                CreatedBy = rolePermission.CreatedBy
            };
            _context.RolePermissions.Add(rolePermissionDb);
            await _context.SaveChangesAsync();
            return rolePermissionDb.Id;
        }

        public async Task<Pagination<RolePermission>> GetAllRolePermissionsAsync(int? roleId, int? actionId, int? statusId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.RolePermissions.AsQueryable();
            if (roleId.HasValue)
            {
                query = query.Where(rp => rp.RoleId == roleId.Value);
            }
            if (actionId.HasValue)
            {
                query = query.Where(rp => rp.ActionId == actionId.Value);
            }
            if (statusId.HasValue)
            {
                query = query.Where(rp => rp.StatusId == statusId.Value);
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .Include(rp => rp.Status)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(rp => new RolePermission
                {
                    Id = rp.Id,
                    RoleId = rp.RoleId,
                    ActionId = rp.ActionId,
                    StatusId = rp.StatusId,
                    CreatedDate = rp.CreatedDate,
                    CreatedBy = rp.CreatedBy,
                    UpdatedDate = rp.UpdatedDate,
                    UpdatedBy = rp.UpdatedBy,
                    Status = rp.Status != null ? new Status
                    {
                        Id = rp.Status.Id,
                        Name = rp.Status.Name
                    } : null
                })
                .ToListAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return new Pagination<RolePermission>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<RolePermission> GetRolePermissionByIdAsync(int id)
        {
            var rp = await _context.RolePermissions
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (rp == null) return null;
            return new RolePermission
            {
                Id = rp.Id,
                RoleId = rp.RoleId,
                ActionId = rp.ActionId,
                StatusId = rp.StatusId,
                Status = rp.Status != null ? new Status
                {
                    Id = rp.Status.Id,
                    Name = rp.Status.Name
                } : null
            };
        }

        public async Task<RolePermission> UpdateRolePermissionAsync(RolePermission rolePermission)
        {
            var existing = await _context.RolePermissions.FindAsync(rolePermission.Id);
            if (existing == null) return null;
            existing.RoleId = rolePermission.RoleId;
            existing.ActionId = rolePermission.ActionId;
            existing.StatusId = rolePermission.StatusId;
            existing.UpdatedDate = rolePermission.UpdatedDate;
            existing.UpdatedBy = rolePermission.UpdatedBy;
            await _context.SaveChangesAsync();
            return rolePermission;
        }

        public async Task<(bool Success, string Message)> DeleteRolePermissionAsync(int id)
        {
            var rp = await _context.RolePermissions.FindAsync(id);
            if (rp == null)
            {
                return (false, "RolePermission not found");
            }
            _context.RolePermissions.Remove(rp);
            await _context.SaveChangesAsync();
            return (true, "RolePermission deleted successfully");
        }

        public async Task<List<string>> GetEffectivePermissionsAsync(long userId)
        {
            var permissions = await _context.Database
                .SqlQueryRaw<string>("SELECT * FROM public.get_effective_permissions({0})", userId)
                .ToListAsync();
            return permissions;
        }
    }
}