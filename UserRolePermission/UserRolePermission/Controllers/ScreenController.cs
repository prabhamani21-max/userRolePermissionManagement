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
    public class ScreenController : ControllerBase
    {
        private readonly ILogger<ScreenController> _logger;
        private readonly IScreenService _screenService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ScreenController(ILogger<ScreenController> logger, IScreenService screenService, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _screenService = screenService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("CreateScreen")]
        public async Task<IActionResult> CreateScreen([FromBody] ScreenDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var screen = _mapper.Map<Screen>(dto);
            screen.CreatedDate = DateTime.UtcNow;
            screen.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            screen.StatusId = (int)GenericStatus.Active;

            var id = await _screenService.CreateScreenAsync(screen);
            _logger.LogInformation("Screen created successfully: {ScreenName}", dto.ScreenName);
            return Ok(new { Message = "Screen created successfully", ScreenId = id });
        }

        [HttpGet("GetAllScreens")]
        public async Task<IActionResult> GetAllScreens(
            [FromQuery] int? statusId = null,
            [FromQuery] string? screenName = null)
        {
            var screens = await _screenService.GetAllScreensAsync(statusId, screenName);

            var response = _mapper.Map<List<ScreenDto>>(screens);

            _logger.LogInformation("Fetched screens successfully");
            return Ok(response);
        }

        [HttpGet("GetScreenById/{id}")]
        public async Task<IActionResult> GetScreenById(int id)
        {
            var screen = await _screenService.GetScreenByIdAsync(id);
            if (screen == null)
            {
                _logger.LogWarning("Screen not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("Screen found with Id: {Id}", id);

            var screenDto = _mapper.Map<ScreenDto>(screen);
            return Ok(screenDto);
        }

        [Authorize]
        [HttpPut("UpdateScreen/{id}")]
        public async Task<IActionResult> UpdateScreen([FromBody] ScreenDto screenDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (screenDto.Id == 0)
            {
                _logger.LogWarning("Screen ID is required for editing.");
                return BadRequest("Screen ID is required.");
            }
            _logger.LogInformation("Editing screen with ID {Id}.", screenDto.Id);
            var screen = _mapper.Map<Screen>(screenDto);
            screen.UpdatedBy = _currentUser.UserId;
            var result = await _screenService.UpdateScreenAsync(screen);
            if (result == null)
            {
                _logger.LogWarning("Screen with ID {Id} not found.", screenDto.Id);
                return NotFound("Screen not found.");
            }

            var resultDto = _mapper.Map<ScreenDto>(result);
            _logger.LogInformation("Updated screen with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("DeleteScreen/{id}")]
        public async Task<IActionResult> DeleteScreen(int id)
        {
            var (success, message) = await _screenService.DeleteScreenAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete screen with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("Screen deleted successfully with Id: {Id}", id);
            return Ok(new { Message = message });
        }
    }
}
