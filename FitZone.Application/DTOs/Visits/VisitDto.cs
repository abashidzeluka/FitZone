using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Application.DTOs.Visits
{
    public class VisitDto
    {
        public int Id { get; set; }
        public DateTime CheckedInAt { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public int? RemainingVisits { get; set; }
    }
}
