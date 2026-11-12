using FitZone.Application.DTOs.Auth;
using FitZone.Application.Interfaces;
using FitZone.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        [HttpPut("update-username")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> UpdateUsername([FromBody] UpdateUsernameDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            var response = await _authService.UpdateUsernameAsync(userId, dto);
            return Ok(response);
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            await _authService.ChangePasswordAsync(userId, dto);
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPost("assign-role/{userId}/{role}")]
        [Authorize(Roles = "Admin,superAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRole(int userId, Role role)
        {
            var isSuperAdmin = User.IsInRole("superAdmin");

            if (!isSuperAdmin && (role == Role.Admin || role == Role.superAdmin))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Access denied: Only SuperAdmin can assign Admin or superAdmin roles."
                });
            }

            var dto = new AssignRoleDto { UserId = userId, Role = role };
            await _authService.AssignRoleAsync(dto);
            return Ok(new { message = $"Role '{role}' assigned successfully to user ID {userId}." });
        }

        [HttpDelete("remove-role/{userId}/{role}")]
        [Authorize(Roles = "Admin,superAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRole(int userId, Role role)
        {
            var isSuperAdmin = User.IsInRole("superAdmin");

            if (!isSuperAdmin && role != Role.Trainer)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Access denied: Admins can only remove the Trainer role."
                });
            }

            var dto = new RemoveRoleDto { UserId = userId, Role = role };
            var result = await _authService.RemoveRoleAsync(dto);

            if (!result)
            {
                return BadRequest(new { message = "Failed to remove the role." });
            }

            return Ok(new { message = $"Role '{role}' was successfully removed from the user." });
        }
    }
}