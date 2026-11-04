using FitZone.Application.DTOs.Auth;
using FitZone.Application.Interfaces;
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
        /// Assigns a specific role to a user (Admin and SuperAdmin only).
        /// </summary>
        /// <param name="dto">Details containing the target user ID and the role to assign.</param>
        /// <returns>A confirmation message if the role assignment was successful.</returns>
        /// <response code="200">Role assigned successfully.</response>
        /// <response code="400">Invalid role assignment operation.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="403">Forbidden - requires Admin or SuperAdmin role privileges.</response>
        /// <response code="404">If the specified user is not found.</response>
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            try
            {
                await _authService.AssignRoleAsync(dto);
                return Ok(new { message = $"Role '{dto.Role}' assigned successfully to user ID {dto.UserId}." });
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