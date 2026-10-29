using FitZone.Domain.Enums;
using System;

namespace FitZone.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int TrainingSessionId { get; set; }
        public TrainingSession TrainingSession { get; set; } = null!;
        public BookingStatus Status { get; set; }
    }
}
