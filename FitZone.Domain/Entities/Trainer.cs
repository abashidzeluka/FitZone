using System;
using System.Collections.Generic;

namespace FitZone.Domain.Entities
{
    public class Trainer
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<TrainingSession> TrainingSessions { get; set; } = new List<TrainingSession>();
    }
}