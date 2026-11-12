using System.ComponentModel.DataAnnotations;

namespace FitZone.Application.DTOs.Trainers
{
    public class UpdateTrainerDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Specialization { get; set; } = string.Empty;
    }
}