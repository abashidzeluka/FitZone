using FitZone.Application.Interfaces;
using FitZone.Application.Services;
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
    }
}
