using FitZone.Application.DTOs.UserMemberships;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// <response code="201">Membership successfully created.</response>
        /// <response code="404">If the specified user or membership plan is not found.</response>
        [HttpPost]
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
        /// <response code="200">Returns the requested membership details.</response>
        /// <response code="404">If the membership does not exist.</response>
        [HttpGet("{id}")]
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
        /// <response code="200">Returns the list of user memberships.</response>
        [HttpGet("user/{userId}")]
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
        /// <response code="404">If the membership is not found.</response>
        [HttpPatch("{id}/cancel")]
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