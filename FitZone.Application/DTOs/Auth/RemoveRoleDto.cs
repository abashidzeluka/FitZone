using FitZone.Domain.Enums;

namespace FitZone.Application.DTOs.Auth
{
    public class RemoveRoleDto
    {
        public int UserId { get; set; }
        public Role Role { get; set; }
    }
}