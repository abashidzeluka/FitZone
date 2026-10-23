using System;

namespace FitZone.Domain.Entities
{
    public class Trainer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    }
}