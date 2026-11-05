using FitZone.Application.DTOs.Auth;
using FitZone.Application.Interfaces;
using FitZone.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Authenticates a user and generates an access token.
        /// </summary>
        /// <param name="dto">User credentials (username and password).</param>
        /// <returns>Authenticated user profile with the generated JWT access token.</returns>
        /// <response code="200">Authentication successful.</response>
        /// <response code="401">Invalid credentials provided.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Registers a new user account in the system.
        /// </summary>
        /// <param name="dto">User registration details including username, email, and password.</param>
        /// <returns>The registered user's profile and an initial JWT token.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Validation error, duplicate username/email, or weak password.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        /// <summary>
        /// Assigns a specific role to a user.
        /// Admins can assign Member or Trainer roles.
        /// Only superAdmins can assign Admin or superAdmin roles.
        /// </summary>
        /// <param name="userId">Unique identifier of the user.</param>
        /// <param name="role">Role enum value to assign.</param>
        /// <returns>A confirmation message if the role assignment was successful.</returns>
        /// <response code="200">Role assigned successfully.</response>
        /// <response code="400">Invalid role assignment operation.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="403">Forbidden - insufficient role hierarchy permissions.</response>
        /// <response code="404">If the specified user is not found.</response>
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

            try
            {
                var dto = new AssignRoleDto { UserId = userId, Role = role };
                await _authService.AssignRoleAsync(dto);
                return Ok(new { message = $"Role '{role}' assigned successfully to user ID {userId}." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Removes a specific role from a user.
        /// Admins can only remove the Trainer role.
        /// superAdmins can remove any assigned role.
        /// </summary>
        /// <param name="userId">Unique identifier of the user.</param>
        /// <param name="role">Role enum value to remove.</param>
        /// <returns>A confirmation message if the role removal was successful.</returns>
        /// <response code="200">Role removed successfully.</response>
        /// <response code="400">User does not have the specified role or operation failed.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="403">Forbidden - insufficient role hierarchy permissions.</response>
        /// <response code="404">If the specified user is not found.</response>
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

            try
            {
                var dto = new RemoveRoleDto { UserId = userId, Role = role };
                var result = await _authService.RemoveRoleAsync(dto);

                if (result)
                {
                    return Ok(new { message = $"Role '{role}' was successfully removed from the user." });
                }

                return BadRequest(new { message = "Failed to remove the role." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}