using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Repository.Interface
{
    public interface IScreenActionRepository
    {
        Task<int> CreateScreenActionAsync(ScreenAction screenAction);
        Task<List<ScreenAction>> GetAllScreenActionsAsync(int? statusId, int? screenId, string? actionName);
        Task<ScreenAction> GetScreenActionByIdAsync(int id);
        Task<ScreenAction> UpdateScreenActionAsync(ScreenAction screenAction);
        Task<(bool Success, string Message)> DeleteScreenActionAsync(int id);
    }
}