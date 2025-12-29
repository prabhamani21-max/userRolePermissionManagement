
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Service.Interface
{
    public interface IMenuStructureService
    {
        Task<int> CreateMenuStructureAsync(MenuStructure menuStructure);
        Task<List<MenuStructure>> GetAllMenuStructuresAsync(bool? isActive, string? label);
        Task<List<MenuStructure>> GetAllRawItemsAsync();
        Task<MenuStructure> GetMenuStructureByIdAsync(int id);
        Task<MenuStructure> UpdateMenuStructureAsync(MenuStructure menuStructure);
        Task<(bool Success, string Message)> DeleteMenuStructureAsync(int id);
    }
}