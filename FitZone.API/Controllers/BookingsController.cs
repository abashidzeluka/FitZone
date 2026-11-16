using FitZone.Application.DTOs.Bookings;
using FitZone.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        /// Books a training session for the currently authenticated user.
        /// </summary>
        /// <param name="dto">Training session booking details.</param>
        /// <returns>The created booking ID and confirmation message.</returns>
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

            var bookingId = await _bookingService.BookSessionAsync(userId, dto);
            return Ok(new { bookingId, message = "Session booked successfully." });
        }

        /// <summary>
        /// Cancels an existing booking if the user is the booking owner or an administrator.
        /// </summary>
        /// <param name="id">Target booking ID.</param>
        /// <returns>Confirmation message upon successful cancellation.</returns>
        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("superAdmin");
            await _bookingService.CancelBookingAsync(id, userId, isAdmin);

            return Ok(new { message = "Booking cancelled successfully." });
        }

        /// <summary>
        /// Retrieves all bookings belonging to the currently authenticated user.
        /// </summary>
        /// <returns>A list of the user's bookings.</returns>
        [HttpGet("my")]
        [ProducesResponseType(typeof(IEnumerable<UserBookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBookings()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user identification." });
            }

            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);
        }

        /// <summary>
        /// Retrieves all bookings across the system (restricted to Admin, superAdmin, and Trainer roles).
        /// </summary>
        /// <returns>A list of all bookings.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,superAdmin,Trainer")]
        [ProducesResponseType(typeof(IEnumerable<UserBookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings);
        }
    }
}