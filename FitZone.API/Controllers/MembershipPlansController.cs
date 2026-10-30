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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var plans = await _planService.GetAllAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plans = await _planService.GetByIdAsync(id);

            if(plans == null)
            {
                return NotFound();
            }   
                return Ok(plans);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMembershipPlanDto dto)
        {
            var createdPlan = await _planService.CreateAsync(dto);

            return Ok(createdPlan);
        }

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
