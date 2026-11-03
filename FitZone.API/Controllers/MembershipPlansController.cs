using FitZone.Application.DTOs.MembershipPlans;
using FitZone.Application.Interfaces;
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
        /// <response code="200">Returns the list of membership plans.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _planService.GetAllAsync();
            return Ok(plans);
        }

        /// <summary>
        /// Retrieves a specific membership plan by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the membership plan.</param>
        /// <response code="200">Returns the requested membership plan.</response>
        /// <response code="404">If the membership plan is not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _planService.GetByIdAsync(id);

            if(plan == null)
            {
                return NotFound();
            }   
                return Ok(plan);
        }

        /// <summary>
        /// Creates a new membership plan.
        /// </summary>
        /// <param name="dto">The details of the membership plan to create.</param>
        /// <response code="201">Returns the newly created membership plan.</response>
        /// <response code="400">If the input data is invalid.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMembershipPlanDto dto)
        {
            var createdPlan = await _planService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = createdPlan.Id }, createdPlan);
        }

        /// <summary>
        /// Deletes a membership plan by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the membership plan to delete.</param>
        /// <response code="204">If the membership plan was successfully deleted.</response>
        /// <response code="404">If the membership plan is not found.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _planService.DeleteAsync(id);

            if(!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
