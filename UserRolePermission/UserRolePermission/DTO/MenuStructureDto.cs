using System.ComponentModel.DataAnnotations;

namespace UserRolePermission.Dto
{
    public class MenuStructureDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Label { get; set; }
        public string? Icon { get; set; }
        public string? RoutePath { get; set; }
        public string? ActionKey { get; set; }
        public bool IsTitle { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsDefaultDashboard { get; set; } = false;

    }
}