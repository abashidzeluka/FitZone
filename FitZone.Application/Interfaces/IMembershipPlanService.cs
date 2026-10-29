using FitZone.Application.DTOs.MembershipPlans;

namespace FitZone.Application.Interfaces;

public interface IMembershipPlanService
{
    Task<IEnumerable<MembershipPlanDto>> GetAllAsync();

    Task<MembershipPlanDto?> GetByIdAsync(int id);

    Task<MembershipPlanDto> CreateAsync(CreateMembershipPlanDto dto);

    Task<bool> DeleteAsync(int id);
}