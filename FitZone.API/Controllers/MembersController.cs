using FitZone.Application.DTOs.Members;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }


        /// <summary>
        /// Retrieves a list of all registered members.
        /// </summary>
        /// <response code="200">Returns the list of members.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll()
        {
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }

        /// <summary>
        /// Retrieves a specific member by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the member.</param>
        /// <response code="200">Returns the requested member.</response>
        /// <response code="404">If the member is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDto>> GetById(int id)
        {
            var member = await _memberService.GetByIdAsync(id);

            if(member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }

        /// <summary>
        /// Creates a new member in the system.
        /// </summary>
        /// <param name="dto">The member registration details.</param>
        /// <response code="201">Returns the newly created member.</response>
        /// <response code="400">If the input data is invalid.</response>
        [HttpPost]
        public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto dto)
        {
            var createdMember = await _memberService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdMember.Id }, createdMember);
        }

        /// <summary>
        /// Deletes a member by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the member to delete.</param>
        /// <response code="204">If the member was successfully deleted.</response>
        /// <response code="404">If the member is not found.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var del = await _memberService.DeleteAsync(id);

            if(!del)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}