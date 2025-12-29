using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Repository.Interface
{
    public interface IMenuStructureRepository
    {
        Task<int> CreateMenuStructureAsync(MenuStructure menuStructure);
        Task<List<MenuStructure>> GetAllMenuStructuresAsync(bool? isActive, string? label);
        Task<List<MenuStructure>> GetAllMenuItemsAsync();
        Task<List<MenuStructure>> GetAllRawItemsAsync();
        Task<MenuStructure> GetMenuStructureByIdAsync(int id);
        Task<MenuStructure> UpdateMenuStructureAsync(MenuStructure menuStructure);
        Task<(bool Success, string Message)> DeleteMenuStructureAsync(int id);
    }
}