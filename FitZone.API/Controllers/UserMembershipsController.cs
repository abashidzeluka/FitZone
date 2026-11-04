using FitZone.Application.DTOs.UserMemberships;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitZone.API.Controllers
{
    /// <summary>
    /// Controller for managing member subscriptions and gym passes.
    /// </summary>
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
        /// Purchases and creates a new membership for a user.
        /// </summary>
        /// <param name="dto">The membership creation details including UserId and PlanId.</param>
        /// <returns>The newly created membership record.</returns>
        /// <response code="201">Membership successfully created.</response>
        /// <response code="400">If the request payload is invalid.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="404">If the specified user or membership plan is not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserMembershipDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        /// <param name="id">The unique identifier of the user membership.</param>
        /// <returns>The requested membership details.</returns>
        /// <response code="200">Returns the requested membership details.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="404">If the membership does not exist.</response>
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
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of memberships owned by the user.</returns>
        /// <response code="200">Returns the list of user memberships.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<UserMembershipDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _service.GetByUserIdAsync(userId);

            return Ok(result);
        }

        /// <summary>
        /// Cancels an active user membership by setting its expiration to the current time.
        /// </summary>
        /// <param name="id">The unique identifier of the membership to cancel.</param>
        /// <response code="204">Membership successfully cancelled.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="404">If the membership is not found.</response>
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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