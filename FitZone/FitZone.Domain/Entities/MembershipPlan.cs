using System;
using System.Collections.Generic;

namespace FitZone.Domain.Entities
{
    public class MembershipPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? VisitLimit { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
    }
}