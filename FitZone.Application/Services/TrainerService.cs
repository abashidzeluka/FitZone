using System;
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

        public async Task<TrainerDto> UpdateTrainerAsync(int trainerId, UpdateTrainerDto dto, int currentUserId, bool isAdmin)
        {
            var trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Id == trainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException($"Trainer with ID {trainerId} not found.");
            }

            if (!isAdmin && trainer.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own trainer profile.");
            }

            trainer.FullName = dto.FullName.Trim();
            trainer.Specialization = dto.Specialization.Trim();

            await _context.SaveChangesAsync();

            return new TrainerDto
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Specialization = trainer.Specialization
            };
        }
    }
}