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
    public class ScreenActionController : ControllerBase
    {
        private readonly ILogger<ScreenActionController> _logger;
        private readonly IScreenActionService _screenActionService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ScreenActionController(ILogger<ScreenActionController> logger, IScreenActionService screenActionService, IMapper mapper, ICurrentUser currentUser)
        {
            _logger = logger;
            _screenActionService = screenActionService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("CreateScreenAction")]
        public async Task<IActionResult> CreateScreenAction([FromBody] ScreenActionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var screenAction = _mapper.Map<ScreenAction>(dto);

            screenAction.CreatedDate = DateTime.UtcNow;
            screenAction.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            screenAction.StatusId = (int)GenericStatus.Active;

            var id = await _screenActionService.CreateScreenActionAsync(screenAction);
            var newScreenAction = await _screenActionService.GetScreenActionByIdAsync(id);
            _logger.LogInformation("ScreenAction created successfully: {ActionName}", dto.ActionName);
            return Ok(new { Message = "ScreenAction created successfully", ScreenActionId = id });
        }

        [Authorize]
        [HttpGet("GetAllScreenActions")]
        public async Task<IActionResult> GetAllScreenActions(
        [FromQuery] int? statusId = null,
        [FromQuery] int? screenId = null,
        [FromQuery] string? actionName = null)
        {
            var screenActions = await _screenActionService.GetAllScreenActionsAsync(statusId, screenId, actionName);

            var response = _mapper.Map<List<ScreenActionDto>>(screenActions);

            _logger.LogInformation("Fetched screen actions successfully");

            return Ok(response);
        }

        [HttpGet("GetScreenActionById/{id}")]
        public async Task<IActionResult> GetScreenActionById(int id)
        {
            var screenAction = await _screenActionService.GetScreenActionByIdAsync(id);
            if (screenAction == null)
            {
                _logger.LogWarning("ScreenAction not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("ScreenAction found with Id: {Id}", id);

            var screenActionDto = _mapper.Map<ScreenActionDto>(screenAction);

            return Ok(screenActionDto);
        }

        [Authorize]
        [HttpPut("UpdateScreenAction/{id}")]
        public async Task<IActionResult> UpdateScreenAction([FromBody] ScreenActionDto screenActionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (screenActionDto.Id == 0)
            {
                _logger.LogWarning("ScreenAction ID is required for editing.");
                return BadRequest("ScreenAction ID is required.");
            }
            _logger.LogInformation("Editing screen action with ID {Id}.", screenActionDto.Id);
            var screenAction = _mapper.Map<ScreenAction>(screenActionDto);
            screenAction.UpdatedBy = _currentUser.UserId;
            var result = await _screenActionService.UpdateScreenActionAsync(screenAction);
            if (result == null)
            {
                _logger.LogWarning("ScreenAction with ID {Id} not found.", screenActionDto.Id);
                return NotFound("ScreenAction not found.");
            }

            var resultDto = _mapper.Map<ScreenActionDto>(result);
            _logger.LogInformation("Updated screen action with ID {Id}.", resultDto.Id);
            return Ok(resultDto);
        }

        [Authorize]
        [HttpDelete("DeleteScreenAction/{id}")]
        public async Task<IActionResult> DeleteScreenAction(int id)
        {
            var (success, message) = await _screenActionService.DeleteScreenActionAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete screen action with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("ScreenAction deleted successfully with Id: {Id}", id);
            return Ok(new { Message = message });
        }
    }
}
