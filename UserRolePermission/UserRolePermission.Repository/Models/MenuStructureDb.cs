using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserRolePermission.Repository.Models
{
    [Table("menu_structure")]
    public class MenuStructureDb
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [Required]
        [Column("label")]
        public string Label { get; set; }

        [Column("icon")]
        public string? Icon { get; set; }

        [Column("route_path")]
        public string? RoutePath { get; set; }

        [Required]
        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("action_key")]
        public string? ActionKey { get; set; }

        [Column("is_title")]
        public bool IsTitle { get; set; } = false;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_default_dashboard")]
        public bool IsDefaultDashboard { get; set; } = false;

        [Required]
        [Column("created_date")]
        public DateTimeOffset CreatedDate { get; set; }

        [Column("updated_date")]
        public DateTimeOffset? UpdatedDate { get; set; }

        //[Column("created_by")]
        //public long CreatedBy { get; set; }

        //[Column("updated_by")]
        //public long? UpdatedBy { get; set; }

        // Navigation properties
        public virtual MenuStructureDb? Parent { get; set; }

        public virtual ICollection<MenuStructureDb> Children { get; set; } = new List<MenuStructureDb>();

      
    }
}