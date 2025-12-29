using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class MenuService : IMenuService
    {
        private readonly IMenuStructureRepository _menuStructureRepository;
        private readonly IMemoryCache _cache;

        public MenuService(IMenuStructureRepository menuStructureRepository, IMemoryCache cache)
        {
            _menuStructureRepository = menuStructureRepository;
            _cache = cache;
        }

        public async Task<List<MenuStructure>> GetAllMenuItemsAsync()
        {
            var cacheKey = $"MenuItems_{MenuStructureService._cacheVersion}";

            if (_cache.TryGetValue(cacheKey, out List<MenuStructure> menuItems))
            {
                return menuItems;
            }

            menuItems = await _menuStructureRepository.GetAllMenuItemsAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, menuItems, cacheEntryOptions);

            return menuItems;
        }
    }
}