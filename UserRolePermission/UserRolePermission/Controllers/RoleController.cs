using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserRolePermission.Authorization;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Dto;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _roleService = roleService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var role = _mapper.Map<Role>(dto);

            role.CreatedDate = DateTime.UtcNow;
            role.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            role.StatusId = (int)GenericStatus.Active;

            var id = await _roleService.CreateRoleAsync(role);
            var newRole = await _roleService.GetRoleByIdAsync(id);
            _logger.LogInformation("Role created successfully: {Name}", dto.Name);
            return Ok(new { Message = "Role created successfully", RoleId = id });
        }

        [Authorize]
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            _logger.LogInformation("Fetching all roles.");
            var roles = await _roleService.GetAllRoles();
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            _logger.LogInformation("Successfully fetched {Count} roles.", roleDtos.Count());
            return Ok(roleDtos);
        }

        [HttpGet("GetRoleById/{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                _logger.LogWarning("Role not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Role found with Id: {Id}", id);

            var roleDto = _mapper.Map<RoleDto>(role);

            return Ok(roleDto);
        }

        [Authorize]
        [HttpPut("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (roleDto.Id == 0)
            {
                _logger.LogWarning("Role ID is required for editing.");
                return BadRequest("Role ID is required.");
            }
            _logger.LogInformation("Editing role with ID {Id}.", roleDto.Id);
            var role = _mapper.Map<Role>(roleDto);
            role.UpdatedBy = _currentUser.UserId;
            var result = await _roleService.UpdateRoleAsync(role);
            if (result == null)
            {
                _logger.LogWarning("Role with ID {Id} not found.", roleDto.Id);
                return NotFound("Role not found.");
            }

            var resultDto = _mapper.Map<RoleDto>(result);
            _logger.LogInformation("Updated role with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var (success, message) = await _roleService.DeleteRoleAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete role with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("Role deleted successfully with Id: {Id}", id);
            return Ok(new { Message = message });
        }
    }
}
