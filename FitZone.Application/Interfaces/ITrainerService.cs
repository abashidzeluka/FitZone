using System.Collections.Generic;
using System.Threading.Tasks;
using FitZone.Application.DTOs.Trainers;

namespace FitZone.Application.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerDto>> GetAllTrainersAsync();
        Task<TrainerDto> UpdateTrainerAsync(int trainerId, UpdateTrainerDto dto, int currentUserId, bool isAdmin);
    }
}