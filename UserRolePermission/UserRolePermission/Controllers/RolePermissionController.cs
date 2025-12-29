using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Dto;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly ILogger<RolePermissionController> _logger;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public RolePermissionController(ILogger<RolePermissionController> logger, IRolePermissionService rolePermissionService, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _rolePermissionService = rolePermissionService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("CreateRolePermission")]
        public async Task<IActionResult> CreateRolePermission([FromBody] RolePermissionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var rolePermission = _mapper.Map<RolePermission>(dto);

            rolePermission.CreatedDate = DateTime.UtcNow;
            rolePermission.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            rolePermission.StatusId = (int)GenericStatus.Active;

            var id = await _rolePermissionService.CreateRolePermissionAsync(rolePermission);
            var newRolePermission = await _rolePermissionService.GetRolePermissionByIdAsync(id);
            _logger.LogInformation("RolePermission created successfully: {Id}", id);
            return Ok(new { Message = "RolePermission created successfully", RolePermissionId = id });
        }

        [Authorize]
        [HttpGet("GetAllRolePermissions")]
        public async Task<IActionResult> GetAllRolePermissions(
        [FromQuery] int? roleId = null,
        [FromQuery] int? actionId = null,
        [FromQuery] int? statusId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            var paginatedRolePermissions = await _rolePermissionService.GetAllRolePermissionsAsync(roleId, actionId, statusId, pageNumber, pageSize);

            // Map only the Items to DTOs, keep pagination metadata as-is
            var response = new Pagination<RolePermissionDto>
            {
                Items = _mapper.Map<List<RolePermissionDto>>(paginatedRolePermissions.Items),
                TotalCount = paginatedRolePermissions.TotalCount,
                PageNumber = paginatedRolePermissions.PageNumber,
                PageSize = paginatedRolePermissions.PageSize,
                TotalPages = paginatedRolePermissions.TotalPages
            };

            _logger.LogInformation("Fetched role permissions successfully with pagination");

            return Ok(response);
        }

        [HttpGet("GetRolePermissionById/{id}")]
        public async Task<IActionResult> GetRolePermissionById(int id)
        {
            var rolePermission = await _rolePermissionService.GetRolePermissionByIdAsync(id);
            if (rolePermission == null)
            {
                _logger.LogWarning("RolePermission not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("RolePermission found with Id: {Id}", id);

            var rolePermissionDto = _mapper.Map<RolePermissionDto>(rolePermission);

            return Ok(rolePermissionDto);
        }

        [Authorize]
        [HttpPut("UpdateRolePermission")]
        public async Task<IActionResult> UpdateRolePermission([FromBody] RolePermissionDto rolePermissionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (rolePermissionDto.Id == 0)
            {
                _logger.LogWarning("RolePermission ID is required for editing.");
                return BadRequest("RolePermission ID is required.");
            }
            _logger.LogInformation("Editing role permission with ID {Id}.", rolePermissionDto.Id);
            var rolePermission = _mapper.Map<RolePermission>(rolePermissionDto);
            rolePermission.UpdatedBy = _currentUser.UserId;
            rolePermission.UpdatedDate = DateTime.UtcNow;
            var result = await _rolePermissionService.UpdateRolePermissionAsync(rolePermission);
            if (result == null)
            {
                _logger.LogWarning("RolePermission with ID {Id} not found.", rolePermissionDto.Id);
                return NotFound("RolePermission not found.");
            }

            var resultDto = _mapper.Map<RolePermissionDto>(result);
            _logger.LogInformation("Updated role permission with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("DeleteRolePermission/{id}")]
        public async Task<IActionResult> DeleteRolePermission(int id)
        {
            var (success, message) = await _rolePermissionService.DeleteRolePermissionAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete role permission with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("RolePermission deleted successfully with Id: {Id}", id);
            return Ok(new { Message = message });
        }
    }
}
