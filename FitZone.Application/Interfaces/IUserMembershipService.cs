using FitZone.Application.DTOs.UserMemberships;

namespace FitZone.Application.Interfaces
{
    public interface IUserMembershipService
    {
        Task<UserMembershipDto> CreateAsync(CreateUserMembershipDto dto);
        Task<UserMembershipDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserMembershipDto>> GetByUserIdAsync(int userId);
        Task<bool> CancelAsync(int id);
    }
}