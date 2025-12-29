using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;

namespace UserRolePermission.Service.Interface
{
    public interface IScreenActionService
    {
        Task<int> CreateScreenActionAsync(ScreenAction screenAction);
        Task<List<ScreenAction>> GetAllScreenActionsAsync(int? statusId, int? screenId, string? actionName);
        Task<ScreenAction> GetScreenActionByIdAsync(int id);
        Task<ScreenAction> UpdateScreenActionAsync(ScreenAction screenAction);
        Task<(bool Success, string Message)> DeleteScreenActionAsync(int id);
    }
}