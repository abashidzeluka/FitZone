using System;
using System.Collections.Generic;

namespace FitZone.Domain.Entities
{
    public class TrainingSession
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TrainerId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int Capacity { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public Trainer Trainer { get; set; } = null!;
    }
}