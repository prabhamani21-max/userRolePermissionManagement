using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserRolePermission.Repository.Models
{
    [Table("user_role")]
    public class UserRoleDb
    {
        [Key]
        [Required]
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("user_id")]
        public long UserId { get; set; }
        [Required]
        [Column("role_id")]
        public int RoleId { get; set; }
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
        public virtual RoleDb Role { get; set; }

    }
}
