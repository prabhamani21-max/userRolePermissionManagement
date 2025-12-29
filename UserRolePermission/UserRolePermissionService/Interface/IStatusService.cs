using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserRolePermission.Common.Models;

namespace UserRolePermission.Service.Interface
{
    public interface IStatusService
    {
        Task<int> CreateStatusAsync(Status status);
        Task<List<Status>> GetAllStatusesAsync();
        Task<Status> GetStatusByIdAsync(int id);
        Task<Status> UpdateStatusAsync(Status status);
        Task<(bool Success, string Message)> DeleteStatusAsync(int id);
    }
}
