
namespace UserRolePermission.Common.Models
{
    public class UserRole
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public virtual Status Status { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
