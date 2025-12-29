using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserRolePermission.Authentication;
using UserRolePermission.Common.Enum;
using UserRolePermission.Common.Models;
using UserRolePermission.Common.Pagination;
using UserRolePermission.Dto;
using UserRolePermission.Helpers;
using UserRolePermission.Repository.Models;
using UserRolePermission.Service.Interface;

namespace UserRolePermission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly JwtTokenGenerator _tokenService;
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IUserPublisherService _userPublisherService;

        public UserController(JwtTokenGenerator tokenService, ILogger<UserController> logger, IUserService userService, IMapper mapper, ICurrentUser currentUser, IUserPublisherService userPublisherService)
        {
            _tokenService = tokenService;
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
            _currentUser = currentUser;
            _userPublisherService = userPublisherService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {

            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var user = await _userService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogError("Enter the proper details");
                return NotFound("Invalid email or password.");
            }
            if (user.StatusId == (int)UsersStatus.Inactive || user.StatusId == (int)UsersStatus.Deleted)
            {
                _logger.LogInformation("Login failed - Status is Not Active: {Email}", dto.Email);
                return UnprocessableEntity("User is InActive");
            }

            // Hash the incoming password and compare with DB value
            var hashedInputPassword = PasswordHasher.HashPassword(dto.Password);
            if (user.Password != hashedInputPassword)
            {
                _logger.LogError("Login failed - Incorrect password for: {Email}", dto.Email);
                return NotFound("Invalid email or password.");
            }

            var userDb = _mapper.Map<UserDb>(user);
            var roleIds = await _userService.GetUserRoleIdsAsync(user.Id);
            var token = _tokenService.GenerateToken(userDb, roleIds);
            _logger.LogInformation("Login Successfull");
            return Ok(new
            {
                Token = token,
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Check email existence
            //if (await _userService.EmailExistsAsync(dto.Email))
            //{
            //    // Return conflict with field-specific error
            //    // Middleware will wrap this with Status=false and standard format
            //    ModelState.AddModelError("Email", "Email already registered");
            //    return Conflict(ModelState);
            //}

            //// Check contact number existence
            //if (await _userService.ContactNumberExistsAsync(dto.ContactNo))
            //{
            //    // Return conflict with field-specific error
            //    ModelState.AddModelError("ContactNo", "Contact number already registered");
            //    return Conflict(ModelState);
            //}

            var user = _mapper.Map<User>(dto);

            user.CreatedDate = DateTime.UtcNow;
            user.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            user.StatusId = (int)GenericStatus.Active;
            user.Password = PasswordHasher.HashPassword(user.Password);

            var id = await _userService.RegisterUserAsync(user);
            var newUser = await _userService.GetUserByIdAsync(id);
            // var userDto = _mapper.Map<UserDto>(newUser);
            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
            await _userPublisherService.PublishUserAdded(newUser);
            return Ok(new { Message = "User registered successfully", UserId = id });
        }

        [Authorize]
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUsers(
        [FromQuery] int? statusId = null,
        [FromQuery] string? name = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            var paginatedUsers = await _userService.GetAllUsersAsync( statusId, name, pageNumber, pageSize);

            // Map only the Items to DTOs, keep pagination metadata as-is
            var response = new Pagination<UserDto>
            {
                Items = _mapper.Map<List<UserDto>>(paginatedUsers.Items),
                TotalCount = paginatedUsers.TotalCount,
                PageNumber = paginatedUsers.PageNumber,
                PageSize = paginatedUsers.PageSize,
                TotalPages = paginatedUsers.TotalPages
            };

            _logger.LogInformation("Fetched users successfully with pagination");

            return Ok(response);
        }



        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found with Id: {Id}", id);
                return NotFound();
            }
            _logger.LogInformation("User  found with Id: {Id}", id);

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [Authorize]
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userDto.Id == 0)
            {
                _logger.LogWarning("User ID is required for editing.");
                return BadRequest("User ID is required.");
            }
            _logger.LogInformation("Editing user with ID {Id}.", userDto.Id);
            var user = _mapper.Map<User>(userDto);
            user.UpdatedBy = _currentUser.UserId;
            var result = await _userService.UpdateUserAsync(user);
            if (result == null)
            {
                _logger.LogWarning("Applicant with ID {Id} not found.", userDto.Id);
                return NotFound("Applicant not found.");
            }

            var resultDto = _mapper.Map<UserDto>(result);
            _logger.LogInformation("Updated applicant with ID {Id}.", resultDto.Id);
            await _userPublisherService.PublishUserUpdated(user);
            return Ok(resultDto);
        }


        [Authorize]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var (success, message) = await _userService.DeleteUserAsync(id);
            if (!success)
            {
                _logger.LogWarning("Failed to delete user with Id: {Id}. Reason: {Message}", id, message);
                return BadRequest(new { Message = message });
            }

            _logger.LogInformation("User deleted successfully with Id: {Id}", id);
            await _userPublisherService.PublishUserDeleted(id);
            return Ok(new { Message = message });
        }
    }
}
