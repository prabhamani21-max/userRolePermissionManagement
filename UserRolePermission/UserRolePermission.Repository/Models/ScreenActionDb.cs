
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace UserRolePermission.Repository.Models
{
    [Table("screen_action")]
    public class ScreenActionDb
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("screen_id")]
        public int ScreenId { get; set; }
        [Required]
        [Column("action_name")]
        public string ActionName { get; set; }
        [Required]
        [Column("key")]
        public string Key { get; set; }
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
        public virtual ScreenDb Screen { get; set; }

    }
}
