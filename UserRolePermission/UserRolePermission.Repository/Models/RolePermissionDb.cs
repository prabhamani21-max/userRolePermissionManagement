
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace UserRolePermission.Repository.Models
{
    [Table("role_permission")]
    public class RolePermissionDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("action_id")]
        public int ActionId { get; set; }
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
        public virtual RoleDb Role { get; set; }
        public virtual ScreenActionDb Action { get; set; }
    }
}
