using System;

namespace FitZone.Application.DTOs.TrainingSessions
{
    public class CreateTrainingSessionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TrainerId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public int Capacity { get; set; }
    }
}