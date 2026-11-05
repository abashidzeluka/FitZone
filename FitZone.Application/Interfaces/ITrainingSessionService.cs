using System.Collections.Generic;
using System.Threading.Tasks;
using FitZone.Application.DTOs.TrainingSessions;

namespace FitZone.Application.Interfaces
{
    public interface ITrainingSessionService
    {
        Task<IEnumerable<TrainingSessionDto>> GetUpcomingSessionsAsync();
        Task<TrainingSessionDto> CreateAsync(CreateTrainingSessionDto dto);
    }
}