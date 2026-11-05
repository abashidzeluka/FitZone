using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FitZone.Application.DTOs.Trainers;

namespace FitZone.Application.Interfaces
{
    public interface ITrainerService
    {
       Task<IEnumerable<TrainerDto>> GetAllTrainersAsync();
    }
}
