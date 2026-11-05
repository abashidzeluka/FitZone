using FitZone.Application.DTOs.TrainingSessions;
using FitZone.Application.Interfaces;
using FitZone.Domain.Entities;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    Title = s.Title,
                    Description = s.Description,
                    TrainerId = s.TrainerId,
                    TrainerName = s.Trainer.FullName,
                    StartsAt = s.StartsAt,
                    EndsAt = s.EndsAt,
                    Capacity = s.Capacity,
                    AvailableSpots = s.Capacity - s.Bookings.Count
                })
                .ToListAsync();
        }

        public async Task<TrainingSessionDto> CreateAsync(CreateTrainingSessionDto dto)
        {
            if (dto.StartsAt <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Session start time must be in the future.");
            }

            if (dto.EndsAt <= dto.StartsAt)
            {
                throw new InvalidOperationException("Session end time must be after the start time.");
            }

            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == dto.TrainerId);
            if (trainer == null)
            {
                throw new InvalidOperationException("Trainer not found.");
            }

            var session = new TrainingSession
            {
                Title = dto.Title,
                Description = dto.Description,
                StartsAt = dto.StartsAt,
                EndsAt = dto.EndsAt,
                Capacity = dto.Capacity,
                TrainerId = dto.TrainerId
            };

            _context.TrainingSessions.Add(session);
            await _context.SaveChangesAsync();

            return new TrainingSessionDto
            {
                Id = session.Id,
                Title = session.Title,
                Description = session.Description,
                TrainerId = session.TrainerId,
                TrainerName = trainer.FullName,
                StartsAt = session.StartsAt,
                EndsAt = session.EndsAt,
                Capacity = session.Capacity,
                AvailableSpots = session.Capacity
            };
        }
    }
}