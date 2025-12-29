
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRolePermission.Repository.Models
{
    [Table("user")]
    public class UserDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required, MaxLength(100)]
        [Column("username")]
        public string Name { get; set; }

        [Required, MaxLength(320)]
        [Column("email")]
        public string Email { get; set; }

        [Required, MaxLength(2000)]
        [Column("password_hash")]
        public string Password { get; set; }

        [Column("status")]
        public int StatusId { get; set; }

        [MaxLength(15)]
        [Column("contact_no")]
        public string ContactNo { get; set; }
        [Column("default_role_id")]
        public int DefaultRoleId { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        // 🔗 Foreign key to Status table
        public virtual StatusDb Status { get; set; }
        public virtual RoleDb DefaultRole { get; set; }

    }
}
