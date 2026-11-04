using FitZone.Application.DTOs.Auth;
using FitZone.Application.Interfaces;
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
    }
}
