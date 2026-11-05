using System.Threading.Tasks;
using FitZone.Application.DTOs.Bookings;

namespace FitZone.Application.Interfaces
{
    public interface IBookingService
    {
        Task<int> BookSessionAsync(int userId, BookingDto dto);
    }
}