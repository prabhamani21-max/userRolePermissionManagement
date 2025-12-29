using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Repository.Data;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Repository.Models;

namespace UserRolePermission.Repository.Implementation
{
    public class UserPermissionOverrideRepository : IUPORepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        public UserPermissionOverrideRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<long> CreateUserPermissionOverrideAsync(UserPermissionOverride userPermissionOverride)
        {
            var entity = _mapper.Map<UserPermissionOverrideDb>(userPermissionOverride);
            await _context.UserPermisssionOverride.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<Pagination<UserPermissionOverride>> GetAllUserPermissionOverridesAsync(int? statusId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.UserPermisssionOverride
                .AsNoTracking()
                .Include(u => u.Status)
                .Include(u => u.User)
                .Include(u => u.Action)
                .AsQueryable();

            if (statusId.HasValue)
            {
                query = query.Where(u => u.StatusId == statusId.Value);
            }

            var totalCount = await query.CountAsync();
            var userPermissionOverrides = await query
                .OrderByDescending(u => u.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new Pagination<UserPermissionOverride>
            {
                Items = _mapper.Map<List<UserPermissionOverride>>(userPermissionOverrides),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<UserPermissionOverride> GetUserPermissionOverrideByIdAsync(long id)
        {
            var userPermissionOverride = await _context.UserPermisssionOverride
                .Include(u => u.Status)
                .Include(u => u.User)
                .Include(u => u.Action)
                .FirstOrDefaultAsync(u => u.Id == id);
            return _mapper.Map<UserPermissionOverride>(userPermissionOverride);
        }

        public async Task<UserPermissionOverride> UpdateUserPermissionOverrideAsync(UserPermissionOverride userPermissionOverride)
        {
            var existingEntity = await _context.UserPermisssionOverride.FindAsync(userPermissionOverride.Id);
            if (existingEntity == null) return null;

            existingEntity.UserId = userPermissionOverride.UserId;
            existingEntity.ActionId = userPermissionOverride.ActionId;
            existingEntity.IsDenied = userPermissionOverride.IsDenied;
            existingEntity.StatusId = userPermissionOverride.StatusId;
            existingEntity.UpdatedDate = DateTime.UtcNow;
            existingEntity.UpdatedBy = userPermissionOverride.UpdatedBy;

            _context.UserPermisssionOverride.Update(existingEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserPermissionOverride>(existingEntity);
        }

        public async Task<(bool Success, string Message)> DeleteUserPermissionOverrideAsync(long id)
        {
            var entity = await _context.UserPermisssionOverride.FindAsync(id);
            if (entity == null) return (false, "User Permission Override not found");

            entity.StatusId = (int)UsersStatus.Deleted;
            _context.UserPermisssionOverride.Update(entity);
            await _context.SaveChangesAsync();
            return (true, "User Permission Override deleted successfully");
        }
    }
}
