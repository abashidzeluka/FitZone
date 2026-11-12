using FitZone.Application.DTOs.Bookings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitZone.Application.Interfaces
{
    public interface IBookingService
    {
        Task<int> BookSessionAsync(int userId, BookingDto dto);
        Task CancelBookingAsync(int bookingId, int userId, bool isAdmin);
        Task<IEnumerable<UserBookingDto>> GetUserBookingsAsync(int userId);
        Task<IEnumerable<UserBookingDto>> GetAllBookingsAsync();
    }
}