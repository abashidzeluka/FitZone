using FitZone.Domain.Enums;

namespace FitZone.Application.DTOs.Auth
{
    public class AssignRoleDto
    {
        public int UserId { get; set; }
        public Role Role { get; set; }
    }
}