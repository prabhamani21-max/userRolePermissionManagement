using System.ComponentModel.DataAnnotations;

namespace UserRolePermission.Dto
{
    public class UserRoleDto
    {
        public long Id { get; set; }
        [Required]
        public long UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public int StatusId { get; set; }
    }
}