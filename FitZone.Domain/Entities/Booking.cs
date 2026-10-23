using FitZone.Domain.Enums;
using System;

namespace FitZone.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TrainingSessionId { get; set; }
        public BookingStatus Status { get; set; }
    }
}
