using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;
using System.Security.Claims;

namespace Mirai.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenGenerator _tokenBlacklistService;
        public UserController(IUserService userService, IJwtTokenGenerator tokenBlacklistService)
        {
            _userService = userService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _userService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userDto)
        {
            try
            {
                var isRegistered = await _userService.RegisterAsync(userDto);
                if (!isRegistered)
                {
                    return BadRequest("User with the same email or user ID already exists.");
                }
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login-user-by-supabase")]
        [Authorize]
        public async Task<IActionResult> SyncUser()
        {
            var uid =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            var email =
                User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value;

            var fullName = User.FindFirst("full_name")?.Value;
            if (string.IsNullOrEmpty(fullName))
            {
                var userMetadataClaim = User.FindFirst("user_metadata");
                if (userMetadataClaim != null && !string.IsNullOrEmpty(userMetadataClaim.Value))
                {
                    try
                    {
                        var metadata = System.Text.Json.JsonDocument.Parse(userMetadataClaim.Value);
                        if (metadata.RootElement.TryGetProperty("full_name", out var fullNameElement))
                        {
                            fullName = fullNameElement.GetString();
                        }
                    }
                    catch (System.Text.Json.JsonException)
                    {
                    }
                }
            }
            var avatarUrl = User.FindFirst("avatar_url")?.Value;
             if (string.IsNullOrEmpty(avatarUrl))
            {
                var userMetadataClaim = User.FindFirst("user_metadata");
                if (userMetadataClaim != null && !string.IsNullOrEmpty(userMetadataClaim.Value))
                {
                    try
                    {
                        var metadata = System.Text.Json.JsonDocument.Parse(userMetadataClaim.Value);
                        if (metadata.RootElement.TryGetProperty("avatar_url", out var avatarUrlElement))
                        {
                            avatarUrl = avatarUrlElement.GetString();
                        }
                    }
                    catch (System.Text.Json.JsonException)
                    {
                    }
                }
            }

            if (string.IsNullOrEmpty(uid))
            {
                return Unauthorized();
            }

            var dto = new SyncSupabaseUserDto
            {
                SupabaseUid = uid,
                Email = email ?? "",
                FullName = fullName ?? "",
                AvatarUrl = avatarUrl ?? ""
            };

            await _userService.SyncSupabaseUserAsync(dto);

            return Ok(new
            {
                message = "Sync success"
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader))
            {
                return BadRequest();
            }

            var token = authHeader.Replace("Bearer ", "");

            await _tokenBlacklistService.BlacklistTokenAsync(token);

            return Ok(new
            {
                message = "Logout successful"
            });
        }

        [Authorize(Roles = "1")]
        [HttpGet("Get-All-Users")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);

        }
        [HttpGet("Get-User-By-UserId{userId}")]
        public async Task<IActionResult> GetAllUsersAsync(string userId)
        {
            var users = await _userService.GetUserByIdAsync(userId);
            return Ok(users);

        }
    }
}
