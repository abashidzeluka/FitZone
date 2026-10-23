using System;

namespace FitZone.Domain.Entities
{
    public class Visit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MembershipId { get; set; }
        public DateTime CheckedInAt { get; set; }
        public User User { get; set; } = null!;
        public UserMembership UserMembership { get; set; } = null!;
    }
}