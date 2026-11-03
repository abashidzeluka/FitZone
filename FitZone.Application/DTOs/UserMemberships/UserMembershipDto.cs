namespace FitZone.Application.DTOs.UserMemberships
{
    public class UserMembershipDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? RemainingVisits { get; set; }
        public bool IsActive { get; set; }
    }
}