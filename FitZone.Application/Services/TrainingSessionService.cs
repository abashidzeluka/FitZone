using FitZone.Application.DTOs.TrainingSessions;
using FitZone.Application.Interfaces;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Application.Services
{
    public class TrainingSessionService : ITrainingSessionService
    {
        private readonly FitZoneDbContext _context;
        public TrainingSessionService(FitZoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrainingSessionDto>> GetUpcomingSessionsAsync()
        {
            return await _context.TrainingSessions
                .Where(s => s.StartsAt > DateTime.UtcNow)
                .AsNoTracking()
                .Select(s => new TrainingSessionDto
                {
                    Id = s.Id,
                    TrainerId = s.TrainerId,
                    TrainerName = s.Trainer.FullName,
                    StartsAt = s.StartsAt,
                    EndsAt = s.EndsAt,
                    Capacity = s.Capacity,
                    AvailableSpots = s.Capacity - s.Bookings.Count
                })
                .ToListAsync();
        }
    }
}
