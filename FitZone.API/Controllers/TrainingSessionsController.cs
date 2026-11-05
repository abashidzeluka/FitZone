using FitZone.Application.DTOs.TrainingSessions;
using FitZone.Application.Interfaces;
using FitZone.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingSessionsController : ControllerBase
    {
        private readonly ITrainingSessionService _trainingSessionService;

        public TrainingSessionsController(ITrainingSessionService trainingsessionservice)
        {
            _trainingSessionService = trainingsessionservice;
        }

        /// <summary>
        /// Retrieves all upcoming training sessions along with trainer details and available spot counts.
        /// </summary>
        /// <returns>A list of upcoming training sessions.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUpcomingSessionsAsync()
        {
            var session = await _trainingSessionService.GetUpcomingSessionsAsync();

            return Ok(session);
        }

        /// <summary>
        /// Creates a new training session (Admin and SuperAdmin only).
        /// </summary>
        /// <param name="dto">Details for the new training session.</param>
        /// <returns>The newly created training session.</returns>
        /// <response code="201">Session was created successfully.</response>
        /// <response code="400">Invalid parameters or schedule conflict.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden - requires Admin or SuperAdmin role.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,superAdmin")]
        [ProducesResponseType(typeof(TrainingSessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateTrainingSessionDto dto)
        {
            try
            {
                var createdSession = await _trainingSessionService.CreateAsync(dto);
                return Created(string.Empty, createdSession);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
