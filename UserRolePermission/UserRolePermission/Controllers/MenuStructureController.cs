using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Dto;
using UserRolePermission.DTO;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuStructureController : ControllerBase
    {
        private readonly IMenuStructureService _menuStructureService;
        private readonly IRolePermissionService _permissionService;
        private readonly IMapper _mapper;

        public MenuStructureController(
            IMenuStructureService menuStructureService,
            IRolePermissionService permissionService,
            IMapper mapper)
        {
            _menuStructureService = menuStructureService;
            _permissionService = permissionService;
            _mapper = mapper;
        }

        // --- NAVIGATION ENDPOINT (For Every User) ---
        [Authorize]
        [HttpGet("GetSidebarMenu")]
        public async Task<IActionResult> GetSidebarMenu()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(userIdClaim, out var userId)) return Unauthorized();

            // 1. Get Cached permissions (Role + Overrides)
            var userPermissions = await _permissionService.GetEffectivePermissionsAsync(userId);

            // 2. Get All Raw Menu Items
            var allItems = await _menuStructureService.GetAllRawItemsAsync();

            // 3. Build filtered hierarchy
            var filteredMenu = BuildMenuHierarchy(allItems, userPermissions, null);

            return Ok(filteredMenu);
        }

        // --- CRUD ENDPOINTS (For Admin Management) ---
        [Authorize] 
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null, [FromQuery] string? label = null)
        {
            var data = await _menuStructureService.GetAllMenuStructuresAsync(isActive, label);
            return Ok(data);
        }

        [Authorize] // Should ideally be [HasPermission("MenuManagement:Read")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _menuStructureService.GetMenuStructureByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuStructureDto menuStructureDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var menuStructure = _mapper.Map<MenuStructure>(menuStructureDto);
            menuStructure.CreatedDate = DateTimeOffset.UtcNow;
            var id = await _menuStructureService.CreateMenuStructureAsync(menuStructure);
            return CreatedAtAction(nameof(GetById), new { id }, new { Id = id });
            // Remove CreatedAtAction Here
        }

        [Authorize] // Should ideally be [HasPermission("MenuManagement:Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MenuStructureDto menuStructureDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var existing = await _menuStructureService.GetMenuStructureByIdAsync(id);
            if (existing == null) return NotFound();
            var menuStructure = _mapper.Map<MenuStructure>(menuStructureDto);
            menuStructure.Id = id;
            menuStructure.UpdatedDate = DateTimeOffset.UtcNow;
            var updated = await _menuStructureService.UpdateMenuStructureAsync(menuStructure);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [Authorize] // Should ideally be [HasPermission("MenuManagement:Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _menuStructureService.DeleteMenuStructureAsync(id);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Message);
        }

        // --- RECURSIVE HELPER ---
        private List<MenuItemDto> BuildMenuHierarchy(List<MenuStructure> items, List<string> permissions, long? parentId)
        {
            return items
                .Where(x => x.ParentId == parentId)
                .Select(x => {
                    var dto = _mapper.Map<MenuItemDto>(x);
                    // Recursively find children
                    dto.SubMenu = BuildMenuHierarchy(items, permissions, x.Id);
                    return dto;
                })
                .Where(dto =>
                    // Security Check: Keep if item has permission key OR if it's a folder with visible children
                    (!string.IsNullOrEmpty(dto.Key) && permissions.Contains(dto.Key)) ||
                    (string.IsNullOrEmpty(dto.Key) && dto.SubMenu.Any())
                )
                .ToList();
        }
    }
}