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
    public class MenuStructureRepository : IMenuStructureRepository
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        public MenuStructureRepository(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> CreateMenuStructureAsync(MenuStructure menuStructure)
        {
            var entity = _mapper.Map<MenuStructureDb>(menuStructure);
            await _context.MenuStructures.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task<List<MenuStructure>> GetAllMenuStructuresAsync(bool? isActive, string? label)
        {
            var query = _context.MenuStructures
                .AsNoTracking()
                .AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(m => m.IsActive == isActive.Value);
            }
            // Filter by label
            if (!string.IsNullOrWhiteSpace(label))
            {
                query = query.Where(m => m.Label.Contains(label));
            }
            var menuStructures = await query
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            return _mapper.Map<List<MenuStructure>>(menuStructures);
        }
        public async Task<List<MenuStructure>> GetAllMenuItemsAsync()
        {
            var menuStructures = await _context.MenuStructures
                .AsNoTracking()
                .Where(m => m.IsActive == true)
                .OrderBy(m => m.SortOrder)
                .ToListAsync();

            return _mapper.Map<List<MenuStructure>>(menuStructures);
        }

        public async Task<List<MenuStructure>> GetAllRawItemsAsync()
        {
            var menuStructures = await _context.MenuStructures
                .Where(x => x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            return _mapper.Map<List<MenuStructure>>(menuStructures);
        }
        public async Task<MenuStructure> GetMenuStructureByIdAsync(int id)
        {
            var menuStructure = await _context.MenuStructures.FindAsync(id);
            return _mapper.Map<MenuStructure>(menuStructure);
        }
        public async Task<MenuStructure> UpdateMenuStructureAsync(MenuStructure menuStructure)
        {
            var existingMenuStructure = await _context.MenuStructures.FindAsync(menuStructure.Id);
            if (existingMenuStructure == null) return null;
            existingMenuStructure.ParentId = menuStructure.ParentId;
            existingMenuStructure.Label = menuStructure.Label;
            existingMenuStructure.Icon = menuStructure.Icon;
            existingMenuStructure.RoutePath = menuStructure.RoutePath;
            existingMenuStructure.SortOrder = menuStructure.SortOrder;
            existingMenuStructure.ActionKey = menuStructure.ActionKey;
            existingMenuStructure.IsTitle = menuStructure.IsTitle;
            existingMenuStructure.IsActive = menuStructure.IsActive;
            existingMenuStructure.IsDefaultDashboard = menuStructure.IsDefaultDashboard;
            existingMenuStructure.UpdatedDate = DateTimeOffset.UtcNow;
            //existingMenuStructure.UpdatedBy = menuStructure.UpdatedBy;

            _context.MenuStructures.Update(existingMenuStructure);
            await _context.SaveChangesAsync();
            return new MenuStructure
            {
                Id = menuStructure.Id,
                ParentId = menuStructure.ParentId,
                Label = menuStructure.Label,
                Icon = menuStructure.Icon,
                RoutePath = menuStructure.RoutePath,
                SortOrder = menuStructure.SortOrder,
                ActionKey = menuStructure.ActionKey,
                IsTitle = menuStructure.IsTitle,
                IsActive = menuStructure.IsActive,
                IsDefaultDashboard = menuStructure.IsDefaultDashboard,
                UpdatedDate = menuStructure.UpdatedDate,
             //   UpdatedBy = menuStructure.UpdatedBy,
            };
        }
        public async Task<(bool Success, string Message)> DeleteMenuStructureAsync(int id)
        {
            var menuStructure = await _context.MenuStructures.FindAsync(id);
            if (menuStructure == null) return (false, "MenuStructure not found");

            menuStructure.IsActive = false;
            _context.MenuStructures.Update(menuStructure);
            await _context.SaveChangesAsync();
            return (true, "MenuStructure deleted successfully");
        }
    }
}