
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRolePermission.Repository.Models
{
    [Table("user_permission_override")]
    public class UserPermissionOverrideDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("user_id")]
        public long UserId { get; set; }
        [Column("action_id")]
        public int ActionId { get; set; }
        [Column("is_denied")]
        public bool IsDenied { get; set; }
        [Required]
        [Column("status")]
        public int StatusId { get; set; }
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }
        [Column("updated_date")]

        public DateTime? UpdatedDate { get; set; }
        public virtual StatusDb Status { get; set; }
        public virtual UserDb User { get; set; } 
        public virtual ScreenActionDb Action { get; set; }
    }
}
