

using UserRolePermission.Service.Interface;

namespace UserRolePermission.Service.Implementation
{
    public class CurrentUser : ICurrentUser
    {
        public long UserId { get; set; }
        public List<string> Roles{ get; set; } 
        public int RoleId { get; set; }
    }

}
