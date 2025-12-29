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
    public class ScreenActionRepository : IScreenActionRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public ScreenActionRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> CreateScreenActionAsync(ScreenAction screenAction)
        {
            var entity = _mapper.Map<ScreenActionDb>(screenAction);
            await _context.ScreenActions.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task<List<ScreenAction>> GetAllScreenActionsAsync(int? statusId, int? screenId, string? actionName)
        {
            var query = _context.ScreenActions
                .AsNoTracking()
                .AsQueryable();

            if (statusId.HasValue)
            {
                query = query.Where(sa => sa.StatusId == statusId.Value);
            }
            if (screenId.HasValue)
            {
                query = query.Where(sa => sa.ScreenId == screenId.Value);
            }
            // Filter by actionName
            if (!string.IsNullOrWhiteSpace(actionName))
            {
                query = query.Where(sa => sa.ActionName.Contains(actionName));
            }
            var screenActions = await query
                .OrderByDescending(sa => sa.Id)
                .ToListAsync();

            return _mapper.Map<List<ScreenAction>>(screenActions);
        }
        public async Task<ScreenAction> GetScreenActionByIdAsync(int id)
        {
            var screenAction = await _context.ScreenActions.FindAsync(id);
            return _mapper.Map<ScreenAction>(screenAction);
        }
        public async Task<ScreenAction> UpdateScreenActionAsync(ScreenAction screenAction)
        {
            var existingScreenAction = await _context.ScreenActions.FindAsync(screenAction.Id);
            if (existingScreenAction == null) return null;
            existingScreenAction.ActionName = screenAction.ActionName;
            existingScreenAction.Key = screenAction.Key;
            existingScreenAction.ScreenId = screenAction.ScreenId;
            existingScreenAction.StatusId = screenAction.StatusId;
            existingScreenAction.UpdatedDate = DateTime.UtcNow;
            existingScreenAction.UpdatedBy = screenAction.UpdatedBy;

            _context.ScreenActions.Update(existingScreenAction);
            await _context.SaveChangesAsync();
            return new ScreenAction
            {
                Id = screenAction.Id,
                ActionName = screenAction.ActionName,
                Key = screenAction.Key,
                ScreenId = screenAction.ScreenId,
                StatusId = screenAction.StatusId,
                UpdatedDate = screenAction.UpdatedDate,
                UpdatedBy = screenAction.UpdatedBy,
            };
        }
        public async Task<(bool Success, string Message)> DeleteScreenActionAsync(int id)
        {
            var screenAction = await _context.ScreenActions.FindAsync(id);
            if (screenAction == null) return (false, "ScreenAction not found");

            screenAction.StatusId = 3; // Deleted
            _context.ScreenActions.Update(screenAction);
            await _context.SaveChangesAsync();
            return (true, "ScreenAction deleted successfully");
        }
    }
}
