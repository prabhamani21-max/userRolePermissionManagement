
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
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public UserRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<long> RegisterUserAsync(User user)
        {
            var entity = _mapper.Map<UserDb>(user);
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task<Pagination<User>> GetAllUsersAsync( int? statusId, string? name, int pageNumber = 1,
    int pageSize = 10)
        {
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

         
            if (statusId.HasValue)
            {
                query = query.Where(u => u.StatusId == statusId.Value);
            }
            // Filter by name
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(u => u.Name.Contains(name));
                // Or case-insensitive:
                // query = query.Where(u => u.Name.ToLower().Contains(name.ToLower()));
            }
            var totalCount = await query.CountAsync();
            var users = await query
          .OrderByDescending(u => u.Id)
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new Pagination<User>
            {
                Items = _mapper.Map<List<User>>(users),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }
        public async Task<User> GetUserByIdAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);
            return _mapper.Map<User>(user);
        }
        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null) return null;
            existingUser.Name = user.Name;
            existingUser.ContactNo = user.ContactNo;
            existingUser.Email = user.Email;
            existingUser.StatusId = user.StatusId;
            existingUser.UpdatedDate = DateTime.UtcNow;
            existingUser.UpdatedBy = user.UpdatedBy;


            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
            return new User
            {
                Id = user.Id,
                Name = user.Name,
                ContactNo = user.ContactNo,
                Email = user.Email,
                StatusId = user.StatusId,
                UpdatedDate = user.UpdatedDate,
                UpdatedBy = user.UpdatedBy,
            };
        }
        public async Task<(bool Success, string Message)> DeleteUserAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return (false, "User not found");
           
            user.StatusId = (int)UsersStatus.Deleted;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return (true, "User deleted successfully");
        }
        public async Task<UserDb> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<List<int>> GetUserRoleIdsAsync(long userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.StatusId == 1) // Assuming 1 is active
                .Select(ur => ur.RoleId)
                .ToListAsync();
        }
    }
}
