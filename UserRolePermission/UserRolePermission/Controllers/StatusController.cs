using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Dto;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public StatusController(ILogger<StatusController> logger, IStatusService statusService, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _statusService = statusService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("CreateStatus")]
        public async Task<IActionResult> CreateStatus([FromBody] StatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var status = _mapper.Map<Status>(dto);

            status.CreatedDate = DateTime.UtcNow;
            status.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var id = await _statusService.CreateStatusAsync(status);
            var newStatus = await _statusService.GetStatusByIdAsync(id);
            _logger.LogInformation("Status created successfully: {Name}", dto.Name);
            return Ok(new { Message = "Status created successfully", StatusId = id });
        }

        [Authorize]
        [HttpGet("GetAllStatuses")]
        public async Task<IActionResult> GetAllStatuses()
        {
            var statuses = await _statusService.GetAllStatusesAsync();

            var statusDtos = _mapper.Map<List<StatusDto>>(statuses);

            _logger.LogInformation("Fetched statuses successfully");

            return Ok(statusDtos);
        }

        [HttpGet("GetStatusById/{id}")]
        public async Task<IActionResult> GetStatusById(int id)
        {
            var status = await _statusService.GetStatusByIdAsync(id);
            if (status == null)
            {
                _logger.LogWarning("Status not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Status found with Id: {Id}", id);

            var statusDto = _mapper.Map<StatusDto>(status);

            return Ok(statusDto);
        }

        [Authorize]
        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus([FromBody] StatusDto statusDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (statusDto.Id == 0)
            {
                _logger.LogWarning("Status ID is required for editing.");
                return BadRequest("Status ID is required.");
            }
            _logger.LogInformation("Editing status with ID {Id}.", statusDto.Id);
            var status = _mapper.Map<Status>(statusDto);
            status.UpdatedBy = _currentUser.UserId;
            var result = await _statusService.UpdateStatusAsync(status);
            if (result == null)
            {
                _logger.LogWarning("Status with ID {Id} not found.", statusDto.Id);
                return NotFound("Status not found.");
            }

            var resultDto = _mapper.Map<StatusDto>(result);
            _logger.LogInformation("Updated status with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("DeleteStatus/{id}")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var (success, message) = await _statusService.DeleteStatusAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete status with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("Status deleted successfully with Id: {Id}", id);
            return Ok(new { Message = message });
        }
    }
}
