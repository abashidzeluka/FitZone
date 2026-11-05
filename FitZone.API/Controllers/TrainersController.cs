using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersController :ControllerBase
    {

        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        /// <summary>
        /// Retrieves the list of all available trainers and their specializations.
        /// </summary>
        /// <returns>A list of trainers.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTrainersAsync()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();

            return Ok(trainers);
        }

    }
}
