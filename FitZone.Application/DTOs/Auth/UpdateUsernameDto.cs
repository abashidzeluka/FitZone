using System.ComponentModel.DataAnnotations;

namespace FitZone.Application.DTOs.Auth;

public class UpdateUsernameDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string NewUserName { get; set; } = string.Empty;
}