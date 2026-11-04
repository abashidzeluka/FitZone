using FitZone.Application.DTOs.MembershipPlans;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipPlansController : ControllerBase
    {
        private readonly IMembershipPlanService _planService;

        public MembershipPlansController(IMembershipPlanService planService)
        {
            _planService = planService;
        }

        /// <summary>
        /// Retrieves a list of all available membership plans.
        /// </summary>
        /// <returns>A list of active membership plans.</returns>
        /// <response code="200">Returns the list of membership plans.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MembershipPlanDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _planService.GetAllAsync();
            return Ok(plans);
        }

        /// <summary>
        /// Retrieves a specific membership plan by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the membership plan.</param>
        /// <returns>The requested membership plan details.</returns>
        /// <response code="200">Returns the requested membership plan.</response>
        /// <response code="404">If the membership plan is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MembershipPlanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _planService.GetByIdAsync(id);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan);
        }

        /// <summary>
        /// Creates a new membership plan (Admin only).
        /// </summary>
        /// <param name="dto">The details of the membership plan to create.</param>
        /// <returns>The newly created membership plan.</returns>
        /// <response code="201">Returns the newly created membership plan.</response>
        /// <response code="400">If the input data is invalid.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="403">Forbidden - requires Admin role privileges.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(MembershipPlanDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateMembershipPlanDto dto)
        {
            var createdPlan = await _planService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = createdPlan.Id }, createdPlan);
        }

        /// <summary>
        /// Deletes a membership plan by its unique identifier (Admin only).
        /// </summary>
        /// <param name="id">The unique identifier of the membership plan to delete.</param>
        /// <response code="204">If the membership plan was successfully deleted.</response>
        /// <response code="401">Unauthorized - valid JWT token is missing or expired.</response>
        /// <response code="403">Forbidden - requires Admin role privileges.</response>
        /// <response code="404">If the membership plan is not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _planService.DeleteAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}