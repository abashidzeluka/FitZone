using FitZone.Application.DTOs.Trainers;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersController : ControllerBase
    {
        private readonly ITrainerService _trainerService;
        private readonly IWebHostEnvironment _env;

        public TrainersController(ITrainerService trainerService, IWebHostEnvironment env)
        {
            _trainerService = trainerService;
            _env = env;
        }

        /// <summary>
        /// Retrieves a list of all registered trainers.
        /// </summary>
        /// <returns>A collection of trainer profiles.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTrainersAsync()
        {
            var trainers = await _trainerService.GetAllTrainersAsync();
            return Ok(trainers);
        }

        /// <summary>
        /// Updates an existing trainer's profile information (restricted to the profile owner or an administrator).
        /// </summary>
        /// <param name="id">Target trainer ID.</param>
        /// <param name="dto">Updated trainer profile details.</param>
        /// <returns>The updated trainer profile.</returns>
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

        /// <summary>
        /// Uploads or replaces a trainer's profile image (restricted to the profile owner or an administrator).
        /// </summary>
        /// <param name="id">Target trainer ID.</param>
        /// <param name="file">Image file (.jpg, .jpeg, .png, or .webp, up to 5MB).</param>
        /// <returns>The relative URL of the uploaded profile image.</returns>
        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "Trainer,Admin,superAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadProfileImageAsync(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "File is required." });
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size cannot exceed 5MB." });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("superAdmin");
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            using var stream = file.OpenReadStream();
            var imageUrl = await _trainerService.UploadProfileImageAsync(id, currentUserId, isAdmin, stream, file.FileName, webRootPath);

            return Ok(new { profileImageUrl = imageUrl });
        }

        /// <summary>
        /// Deletes a trainer's profile image from the server and clears its URL (restricted to the profile owner or an administrator).
        /// </summary>
        /// <param name="id">Target trainer ID.</param>
        /// <returns>No content upon successful deletion.</returns>
        [HttpDelete("{id}/image")]
        [Authorize(Roles = "Trainer,Admin,superAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProfileImageAsync(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var currentUserId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("superAdmin");
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            await _trainerService.DeleteProfileImageAsync(id, currentUserId, isAdmin, webRootPath);

            return NoContent();
        }
    }
}