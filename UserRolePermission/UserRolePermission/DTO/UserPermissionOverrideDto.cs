using System.ComponentModel.DataAnnotations;

namespace UserRolePermission.Dto
{
    public class UserPermissionOverrideDto
    {
        public long Id { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public int ActionId { get; set; }
        [Required]
        public bool IsDenied { get; set; }
        [Required]
        public int StatusId { get; set; }
    }
}