using FitZone.Application.DTOs.Auth;

namespace FitZone.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<bool> AssignRoleAsync(AssignRoleDto dto);
    Task<bool> RemoveRoleAsync(RemoveRoleDto dto);
}