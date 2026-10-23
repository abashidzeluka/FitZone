using System;

namespace FitZone.Domain.Entities
{
    public class UserMembership
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? RemainingVisits { get; set; }
        public User User { get; set; } = null!;
        public MembershipPlan MembershipPlan { get; set; } = null!;
    }
}