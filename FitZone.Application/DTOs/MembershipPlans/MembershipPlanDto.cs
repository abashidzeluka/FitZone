namespace FitZone.Application.DTOs.MembershipPlans;

public class MembershipPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? VisitLimit { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
}