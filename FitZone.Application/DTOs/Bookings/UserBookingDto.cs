using System;

namespace FitZone.Application.DTOs.Bookings
{
    public class UserBookingDto
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TrainingSessionId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}