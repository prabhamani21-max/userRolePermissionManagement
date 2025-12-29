using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Repository.Interface;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class MenuStructureService : IMenuStructureService
    {
        private readonly IMenuStructureRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        public static int _cacheVersion = 0;

        public MenuStructureService(IMenuStructureRepository repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<int> CreateMenuStructureAsync(MenuStructure menuStructure)
        {
            var result = await _repository.CreateMenuStructureAsync(menuStructure);
            Interlocked.Increment(ref _cacheVersion);
            return result;
        }
        public async Task<List<MenuStructure>> GetAllMenuStructuresAsync(bool? isActive, string? label)
        {
            var cacheKey = $"MenuStructures_{isActive}_{label}_{_cacheVersion}";

            if (_cache.TryGetValue(cacheKey, out List<MenuStructure> menuStructures))
            {
                return menuStructures;
            }

            menuStructures = await _repository.GetAllMenuStructuresAsync(isActive, label);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, menuStructures, cacheEntryOptions);

            return menuStructures;
        }

        public async Task<List<MenuStructure>> GetAllRawItemsAsync()
        {
            // Fetches all active menu items without pagination for the sidebar builder
            var cacheKey = $"MenuRawItems_{_cacheVersion}";

            if (_cache.TryGetValue(cacheKey, out List<MenuStructure> menuItems))
            {
                return menuItems;
            }

            menuItems = await _repository.GetAllRawItemsAsync();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(cacheKey, menuItems, cacheEntryOptions);

            return menuItems;
        }

        public async Task<MenuStructure> GetMenuStructureByIdAsync(int id)
        {
            return await _repository.GetMenuStructureByIdAsync(id);
        }
        public async Task<MenuStructure> UpdateMenuStructureAsync(MenuStructure menuStructure)
        {
            var result = await _repository.UpdateMenuStructureAsync(menuStructure);
            if (result != null)
            {
                Interlocked.Increment(ref _cacheVersion);
            }
            return result;
        }

        public async Task<(bool Success, string Message)> DeleteMenuStructureAsync(int id)
        {
            var result = await _repository.DeleteMenuStructureAsync(id);
            if (result.Success)
            {
                Interlocked.Increment(ref _cacheVersion);
            }
            return result;
        }
    }
}