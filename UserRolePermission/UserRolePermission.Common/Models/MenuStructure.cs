using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UserRolePermission.Common.Models
{
    public class MenuStructure
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Label { get; set; }
        public string? Icon { get; set; }
        public string? RoutePath { get; set; }
        public int SortOrder { get; set; } = 0;
        public string? ActionKey { get; set; }
        public bool IsTitle { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsDefaultDashboard { get; set; } = false;
        [JsonIgnore]
        public DateTimeOffset CreatedDate { get; set; }
        [JsonIgnore]
        public DateTimeOffset? UpdatedDate { get; set; }
        //public long CreatedBy { get; set; }
        //public long? UpdatedBy { get; set; }

        // Navigation properties
        public virtual MenuStructure? Parent { get; set; }
        public virtual ICollection<MenuStructure> Children { get; set; } = new List<MenuStructure>();
        public virtual Screen? Screen { get; set; }
        public virtual ScreenAction? ScreenAction { get; set; }
    }
}
