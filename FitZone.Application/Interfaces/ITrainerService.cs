using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FitZone.Application.DTOs.Trainers;

namespace FitZone.Application.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerDto>> GetAllTrainersAsync();
        Task<TrainerDto> UpdateTrainerAsync(int trainerId, UpdateTrainerDto dto, int currentUserId, bool isAdmin);
        Task<string> UploadProfileImageAsync(int trainerId, int currentUserId, bool isAdmin, Stream fileStream, string fileName, string webRootPath);
        Task DeleteProfileImageAsync(int trainerId, int currentUserId, bool isAdmin, string webRootPath);
    }
}