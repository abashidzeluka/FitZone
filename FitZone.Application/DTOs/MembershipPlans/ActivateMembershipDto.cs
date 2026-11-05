using System.ComponentModel.DataAnnotations;

namespace FitZone.Application.DTOs.MembershipPlans
{
    public class ActivateMembershipDto
    {
        [Required]
        public int PlanId { get; set; }
    }
}