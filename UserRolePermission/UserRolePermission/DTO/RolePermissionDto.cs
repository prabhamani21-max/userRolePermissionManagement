using System.ComponentModel.DataAnnotations;

namespace UserRolePermission.Dto
{
    public class RolePermissionDto
    {
        public int Id { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public int ActionId { get; set; }
        [Required]
        public int StatusId { get; set; }
    }
}