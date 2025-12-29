using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Service.Interface
{
    public interface IScreenService
    {
        Task<int> CreateScreenAsync(Screen screen);
        Task<List<Screen>> GetAllScreensAsync(int? statusId, string? screenName);
        Task<Screen> GetScreenByIdAsync(int id);
        Task<Screen> UpdateScreenAsync(Screen screen);
        Task<(bool Success, string Message)> DeleteScreenAsync(int id);
    }
}
