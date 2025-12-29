

using UserRolePermission.Common.Models;

namespace UserRolePermission.Service.Interface
{
    public interface IUserPublisherService
    {
        Task PublishUserAdded(User user);
        Task PublishUserUpdated(User user);
        Task PublishUserDeleted(long userId);
    }
}
