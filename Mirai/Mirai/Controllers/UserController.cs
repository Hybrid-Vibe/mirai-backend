using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Services;

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
