using System.Collections.Generic;

namespace UserRolePermission.DTO
{
    public class MenuItemDto
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public bool Collapsed { get; set; } = true;
        public bool IsTitle { get; set; }
        public string? RoutePath { get; set; }
        public bool IsDefaultDashboard { get; set; } = false;
        public List<MenuItemDto> SubMenu { get; set; } = new();
    }
}