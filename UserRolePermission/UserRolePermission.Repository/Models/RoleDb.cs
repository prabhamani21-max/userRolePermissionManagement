
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRolePermission.Repository.Models
{
   [Table("role")]
    public class RoleDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
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

    }
}
