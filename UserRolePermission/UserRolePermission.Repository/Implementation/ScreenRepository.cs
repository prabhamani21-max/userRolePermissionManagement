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
    public class ScreenRepository : IScreenRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        public ScreenRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> CreateScreenAsync(Screen screen)
        {
            var entity = _mapper.Map<ScreenDb>(screen);
            await _context.Screens.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<List<Screen>> GetAllScreensAsync(int? statusId, string? screenName)
        {
            var query = _context.Screens
                .AsNoTracking()
                .AsQueryable();

            if (statusId.HasValue)
            {
                query = query.Where(s => s.StatusId == statusId.Value);
            }

            if (!string.IsNullOrWhiteSpace(screenName))
            {
                query = query.Where(s => s.ScreenName.Contains(screenName));
            }

            var screens = await query
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            return _mapper.Map<List<Screen>>(screens);
        }

        public async Task<Screen> GetScreenByIdAsync(int id)
        {
            var screen = await _context.Screens.FindAsync(id);
            return _mapper.Map<Screen>(screen);
        }

        public async Task<Screen> UpdateScreenAsync(Screen screen)
        {
            var existingScreen = await _context.Screens.FindAsync(screen.Id);
            if (existingScreen == null) return null;

            existingScreen.ScreenName = screen.ScreenName;
            existingScreen.Key = screen.Key;
            existingScreen.StatusId = screen.StatusId;
            existingScreen.UpdatedDate = DateTime.UtcNow;
            existingScreen.UpdatedBy = screen.UpdatedBy;

            _context.Screens.Update(existingScreen);
            await _context.SaveChangesAsync();

            return new Screen
            {
                Id = screen.Id,
                ScreenName = screen.ScreenName,
                Key = screen.Key,
                StatusId = screen.StatusId,
                UpdatedDate = screen.UpdatedDate,
                UpdatedBy = screen.UpdatedBy,
            };
        }

        public async Task<(bool Success, string Message)> DeleteScreenAsync(int id)
        {
            var screen = await _context.Screens.FindAsync(id);
            if (screen == null) return (false, "Screen not found");

            screen.StatusId = (int)UsersStatus.Deleted;
            _context.Screens.Update(screen);
            await _context.SaveChangesAsync();
            return (true, "Screen deleted successfully");
        }
    }
}
