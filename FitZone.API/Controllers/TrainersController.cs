using FitZone.Application.DTOs.Trainers;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersController : ControllerBase
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrainersAsync()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            return Ok(trainers);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Trainer,Admin,superAdmin")]
        [ProducesResponseType(typeof(TrainerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTrainerAsync(int id, [FromBody] UpdateTrainerDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("superAdmin");
            var updatedTrainer = await _trainerService.UpdateTrainerAsync(id, dto, currentUserId, isAdmin);

            return Ok(updatedTrainer);
        }
    }
}