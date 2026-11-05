using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitZone.Application.DTOs.Trainers;
using FitZone.Application.Interfaces;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitZone.Application.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly FitZoneDbContext _context;

        public TrainerService(FitZoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrainerDto>> GetAllTrainersAsync()
        {
            return await _context.Trainers
                .AsNoTracking()
                .Select(t => new TrainerDto
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    Specialization = t.Specialization
                })
                .ToListAsync();
        }
    }
}