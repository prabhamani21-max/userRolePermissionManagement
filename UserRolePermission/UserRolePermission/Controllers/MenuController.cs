using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserRolePermission.Common.Models;
using UserRolePermission.DTO;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IRolePermissionService _permissionService;

        public MenuController(IMenuService menuService, IRolePermissionService permissionService)
        {
            _menuService = menuService;
            _permissionService = permissionService;
        }

        [HttpGet("sidebar")]
        public async Task<IActionResult> GetUserMenu()
        {
            // 1. Get User ID from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId)) return Unauthorized();

            // 2. Get Cached Effective Permissions
            var userPermissions = await _permissionService.GetEffectivePermissionsAsync(userId);

            // 3. Fetch All Menu Items from Database
            var allMenuItems = await _menuService.GetAllMenuItemsAsync();

            // 4. Build Hierarchical Structure with Security Filtering
            var filteredMenu = BuildMenuHierarchy(allMenuItems, userPermissions, null);

            return Ok(filteredMenu);
        }

        private List<MenuItemDto> BuildMenuHierarchy(List<MenuStructure> items, List<string> permissions, int? parentId)
        {
            return items
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.SortOrder)
                .Select(x => new MenuItemDto
                {
                    Key = x.Label.Replace(" ", "").ToLower(),
                    Label = x.Label,
                    Icon = x.Icon,
                    Link = x.RoutePath,
                    IsTitle = x.IsTitle,
                    // Recursively build submenus
                    SubMenu = BuildMenuHierarchy(items, permissions, x.Id)
                })
                // SECURITY FILTER:
                // Keep item if: 1. It has action_key and the user has that permission
                // OR 2. It has no action_key (folder) but has visible children
                .Where(dto =>
                    (!string.IsNullOrEmpty(dto.Link) && items.Any(i => i.Label == dto.Label && permissions.Contains(i.ActionKey))) ||
                    (string.IsNullOrEmpty(dto.Link) && dto.SubMenu.Any())
                )
                .ToList();
        }
    }
}