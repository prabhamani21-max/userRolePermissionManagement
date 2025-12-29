using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Dto;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UPOController : ControllerBase
    {
        private readonly IUPOService _upoService;
        private readonly ILogger<UPOController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public UPOController(IUPOService upoService, ILogger<UPOController> logger, IMapper mapper, ICurrentUser currentUser)
        {
            _upoService = upoService;
            _logger = logger;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("CreateUserPermissionOverride")]
        public async Task<IActionResult> CreateUserPermissionOverride([FromBody] UserPermissionOverrideDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userPermissionOverride = _mapper.Map<UserPermissionOverride>(dto);
            userPermissionOverride.CreatedDate = DateTime.UtcNow;
            userPermissionOverride.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            userPermissionOverride.StatusId = (int)GenericStatus.Active;

            var id = await _upoService.CreateUserPermissionOverrideAsync(userPermissionOverride);
            _logger.LogInformation("User Permission Override created successfully with Id: {Id}", id);
            return Ok(new { Message = "User Permission Override created successfully", Id = id });
        }

        [HttpGet("GetAllUserPermissionOverrides")]
        public async Task<IActionResult> GetAllUserPermissionOverrides(
            [FromQuery] int? statusId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedUserPermissionOverrides = await _upoService.GetAllUserPermissionOverridesAsync(statusId, pageNumber, pageSize);

            var response = new Pagination<UserPermissionOverrideDto>
            {
                Items = _mapper.Map<List<UserPermissionOverrideDto>>(paginatedUserPermissionOverrides.Items),
                TotalCount = paginatedUserPermissionOverrides.TotalCount,
                PageNumber = paginatedUserPermissionOverrides.PageNumber,
                PageSize = paginatedUserPermissionOverrides.PageSize,
                TotalPages = paginatedUserPermissionOverrides.TotalPages
            };

            _logger.LogInformation("Fetched User Permission Overrides successfully with pagination");
            return Ok(response);
        }

        [HttpGet("GetUserPermissionOverrideById/{id}")]
        public async Task<IActionResult> GetUserPermissionOverrideById(long id)
        {
            var userPermissionOverride = await _upoService.GetUserPermissionOverrideByIdAsync(id);
            if (userPermissionOverride == null)
            {
                _logger.LogWarning("User Permission Override not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("User Permission Override found with Id: {Id}", id);

            var dto = _mapper.Map<UserPermissionOverrideDto>(userPermissionOverride);
            return Ok(dto);
        }

        [Authorize]
        [HttpPut("UpdateUserPermissionOverride/{id}")]
        public async Task<IActionResult> UpdateUserPermissionOverride([FromBody] UserPermissionOverrideDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.Id == 0)
            {
                _logger.LogWarning("User Permission Override ID is required for editing.");
                return BadRequest("User Permission Override ID is required.");
            }

            _logger.LogInformation("Editing User Permission Override with ID {Id}.", dto.Id);
            var userPermissionOverride = _mapper.Map<UserPermissionOverride>(dto);
            userPermissionOverride.UpdatedBy = _currentUser.UserId;
            var result = await _upoService.UpdateUserPermissionOverrideAsync(userPermissionOverride);
            if (result == null)
            {
                _logger.LogWarning("User Permission Override with ID {Id} not found.", dto.Id);
                return NotFound("User Permission Override not found.");
            }

            var resultDto = _mapper.Map<UserPermissionOverrideDto>(result);
            _logger.LogInformation("Updated User Permission Override with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("DeleteUserPermissionOverride/{id}")]
        public async Task<IActionResult> DeleteUserPermissionOverride(long id)
        {
            var (success, message) = await _upoService.DeleteUserPermissionOverrideAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete User Permission Override with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("User Permission Override deleted successfully with Id: {Id}", id);
            return Ok(new { Message = message });
        }
    }
}
