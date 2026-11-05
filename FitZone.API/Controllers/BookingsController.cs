using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FitZone.Application.DTOs.Bookings;

namespace FitZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Books an upcoming training session for the authenticated member.
        /// </summary>
        /// <param name="dto">The session booking payload containing the training session ID.</param>
        /// <returns>Confirmation details including the generated booking identifier.</returns>
        /// <response code="200">The session was successfully booked.</response>
        /// <response code="400">The session is invalid, fully booked, already booked, or the user lacks an active membership.</response>
        /// <response code="401">User is not authorized or token is invalid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BookSession([FromBody] BookingDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            try
            {
                var bookingId = await _bookingService.BookSessionAsync(userId, dto);
                return Ok(new { bookingId, message = "Session booked successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
