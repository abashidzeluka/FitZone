using FitZone.Application.DTOs.UserMemberships;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitZone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class UserMembershipsController : ControllerBase
    {
        private readonly IUserMembershipService _service;

        public UserMembershipsController(IUserMembershipService service)
        {
            _service = service;
        }

        /// <summary>
        /// Activates a new membership plan for the authenticated user (or a specified user if requested by an administrator).
        /// </summary>
        /// <param name="dto">User membership creation details.</param>
        /// <returns>The created user membership details.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserMembershipDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateUserMembershipDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var currentUserId))
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("superAdmin");

            dto.UserId = isAdmin && dto.UserId.HasValue && dto.UserId.Value > 0
                ? dto.UserId.Value
                : currentUserId;

            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Retrieves a specific user membership record by its unique identifier.
        /// </summary>
        /// <param name="id">User membership ID.</param>
        /// <returns>The matching user membership details.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserMembershipDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all memberships belonging to a specific user (restricted to the account owner or an administrator).
        /// </summary>
        /// <param name="userId">Target user ID.</param>
        /// <returns>A list of the user's memberships.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<UserMembershipDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isPrivileged = User.IsInRole("Admin") || User.IsInRole("superAdmin");

            if (!isPrivileged && currentUserId != userId.ToString())
            {
                return Forbid();
            }

            var result = await _service.GetByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Cancels an active user membership (restricted to Admin and superAdmin roles).
        /// </summary>
        /// <param name="id">Target user membership ID.</param>
        /// <returns>No content upon successful cancellation.</returns>
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,superAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _service.CancelAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}