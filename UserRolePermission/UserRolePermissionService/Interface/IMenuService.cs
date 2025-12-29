using System.Collections.Generic;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;

namespace UserRolePermission.Service.Interface
{
    public interface IMenuService
    {
        Task<List<MenuStructure>> GetAllMenuItemsAsync();
    }
}