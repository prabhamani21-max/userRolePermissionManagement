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
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public UserRoleRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<long> CreateUserRoleAsync(UserRole userRole)
        {
            var entity = _mapper.Map<UserRoleDb>(userRole);
            await _context.UserRoles.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task<Pagination<UserRole>> GetAllUserRolesAsync(int? statusId, long? userId, int? roleId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.UserRoles
                .Include(ur => ur.Status)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .AsNoTracking()
                .AsQueryable();

            if (statusId.HasValue)
            {
                query = query.Where(ur => ur.StatusId == statusId.Value);
            }
            if (userId.HasValue)
            {
                query = query.Where(ur => ur.UserId == userId.Value);
            }
            if (roleId.HasValue)
            {
                query = query.Where(ur => ur.RoleId == roleId.Value);
            }
            var totalCount = await query.CountAsync();
            var userRoles = await query
                .OrderByDescending(ur => ur.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new Pagination<UserRole>
            {
                Items = _mapper.Map<List<UserRole>>(userRoles),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
        public async Task<UserRole> GetUserRoleByIdAsync(long id)
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.Status)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.Id == id);
            return _mapper.Map<UserRole>(userRole);
        }
        public async Task<UserRole> UpdateUserRoleAsync(UserRole userRole)
        {
            var existingUserRole = await _context.UserRoles.FindAsync(userRole.Id);
            if (existingUserRole == null) return null;
            existingUserRole.UserId = userRole.UserId;
            existingUserRole.RoleId = userRole.RoleId;
            existingUserRole.StatusId = userRole.StatusId;
            existingUserRole.UpdatedDate = DateTime.UtcNow;
            existingUserRole.UpdatedBy = userRole.UpdatedBy;

            _context.UserRoles.Update(existingUserRole);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserRole>(existingUserRole);
        }
        public async Task<(bool Success, string Message)> DeleteUserRoleAsync(long id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null) return (false, "UserRole not found");

            userRole.StatusId = (int)GenericStatus.Inactive;
            _context.UserRoles.Update(userRole);
            await _context.SaveChangesAsync();
            return (true, "UserRole deleted successfully");
        }
        public async Task<List<UserRole>> GetUserRolesByUserIdAsync(long userId)
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Status)
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .ToListAsync();
            return _mapper.Map<List<UserRole>>(userRoles);
        }
    }
}
