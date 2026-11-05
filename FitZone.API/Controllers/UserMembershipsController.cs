using FitZone.Application.DTOs.UserMemberships;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        /// Purchases and creates a new membership for a user (Admin and superAdmin only).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,superAdmin")]
        [ProducesResponseType(typeof(UserMembershipDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateUserMembershipDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a specific user membership by its unique identifier.
        /// </summary>
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
        /// Retrieves all memberships associated with a specific user.
        /// </summary>
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
        /// Cancels an active user membership by setting its expiration to the current time (Admin and superAdmin only).
        /// </summary>
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