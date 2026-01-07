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
    public class UserRoleController : ControllerBase
    {
        private readonly ILogger<UserRoleController> _logger;
        private readonly IUserRoleService _userRoleService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public UserRoleController(ILogger<UserRoleController> logger, IUserRoleService userRoleService, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _userRoleService = userRoleService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("AssignUserRole")]
        public async Task<IActionResult> AssignUserRole([FromBody] UserRoleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userRole = _mapper.Map<UserRole>(dto);
            userRole.CreatedDate = DateTime.UtcNow;
            userRole.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            userRole.StatusId = (int)GenericStatus.Active;

            var id = await _userRoleService.CreateUserRoleAsync(userRole);
            _logger.LogInformation("UserRole assigned successfully: UserId {UserId}, RoleId {RoleId}", dto.UserId, dto.RoleId);
            return Ok(new { Message = "UserRole assigned successfully", UserRoleId = id });
        }

        [HttpGet("GetAllUserRoles")]
        public async Task<IActionResult> GetAllUserRoles(
            [FromQuery] int? statusId = null,
            [FromQuery] long? userId = null,
            [FromQuery] int? roleId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedUserRoles = await _userRoleService.GetAllUserRolesAsync(statusId, userId, roleId, pageNumber, pageSize);

            var response = new Pagination<UserRoleDto>
            {
                Items = _mapper.Map<List<UserRoleDto>>(paginatedUserRoles.Items),
                TotalCount = paginatedUserRoles.TotalCount,
                PageNumber = paginatedUserRoles.PageNumber,
                PageSize = paginatedUserRoles.PageSize,
                TotalPages = paginatedUserRoles.TotalPages
            };

            _logger.LogInformation("Fetched user roles successfully with pagination");
            return Ok(response);
        }

        [HttpGet("GetUserRoleById/{id}")]
        public async Task<IActionResult> GetUserRoleById(long id)
        {
            var userRole = await _userRoleService.GetUserRoleByIdAsync(id);
            if (userRole == null)
            {
                _logger.LogWarning("UserRole not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("UserRole found with Id: {Id}", id);

            var userRoleDto = _mapper.Map<UserRoleDto>(userRole);
            return Ok(userRoleDto);
        }

        [HttpPut("UpdateUserRole/{id}")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleDto userRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userRoleDto.Id == 0)
            {
                _logger.LogWarning("UserRole ID is required for editing.");
                return BadRequest("UserRole ID is required.");
            }
            _logger.LogInformation("Editing user role with ID {Id}.", userRoleDto.Id);
            var userRole = _mapper.Map<UserRole>(userRoleDto);
            userRole.UpdatedBy = _currentUser.UserId;
            var result = await _userRoleService.UpdateUserRoleAsync(userRole);
            if (result == null)
            {
                _logger.LogWarning("UserRole with ID {Id} not found.", userRoleDto.Id);
                return NotFound("UserRole not found.");
            }

            var resultDto = _mapper.Map<UserRoleDto>(result);
            _logger.LogInformation("Updated user role with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [HttpDelete("DeleteUserRole/{id}")]
        public async Task<IActionResult> DeleteUserRole(long id)
        {
            var (success, message) = await _userRoleService.DeleteUserRoleAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete user role with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("User role deleted successfully with Id: {Id}", id);
            return Ok(new { Message = message });
        }

        [HttpGet("GetUserRolesByUserId/{userId}")]
        public async Task<IActionResult> GetUserRolesByUserId(long userId)
        {
            var userRoles = await _userRoleService.GetUserRolesByUserIdAsync(userId);
            var userRoleDtos = _mapper.Map<List<UserRoleDto>>(userRoles);
            _logger.LogInformation("Fetched user roles for user Id: {UserId}", userId);
            return Ok(userRoleDtos);
        }
    }
}
