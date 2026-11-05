using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Application.DTOs.TrainingSessions
{
    public class TrainingSessionDto
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = string.Empty;
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int Capacity { get; set; }
        public int AvailableSpots { get; set; }
    }
}
