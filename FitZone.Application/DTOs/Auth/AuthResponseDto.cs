using System.Collections.Generic;

namespace FitZone.Application.DTOs.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
}