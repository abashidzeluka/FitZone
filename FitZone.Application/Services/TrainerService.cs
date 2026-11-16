using System;
using System.Collections.Generic;
using System.IO;
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
                    Specialization = t.Specialization,
                    ProfileImageUrl = t.ProfileImageUrl
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
                Specialization = trainer.Specialization,
                ProfileImageUrl = trainer.ProfileImageUrl
            };
        }

        public async Task<string> UploadProfileImageAsync(int trainerId, int currentUserId, bool isAdmin, Stream fileStream, string fileName, string webRootPath)
        {
            var trainer = await _context.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Trainer not found.");
            }

            if (!isAdmin && trainer.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own trainer profile image.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Invalid file format. Only .jpg, .jpeg, .png, and .webp are allowed.");
            }

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "trainers");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (!string.IsNullOrEmpty(trainer.ProfileImageUrl))
            {
                var oldFilePath = Path.Combine(webRootPath, trainer.ProfileImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var destinationStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(destinationStream);
            }

            trainer.ProfileImageUrl = $"/uploads/trainers/{uniqueFileName}";
            await _context.SaveChangesAsync();

            return trainer.ProfileImageUrl;
        }

        public async Task DeleteProfileImageAsync(int trainerId, int currentUserId, bool isAdmin, string webRootPath)
        {
            var trainer = await _context.Trainers.FindAsync(trainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Trainer not found.");
            }

            if (!isAdmin && trainer.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own trainer profile image.");
            }

            if (string.IsNullOrEmpty(trainer.ProfileImageUrl))
            {
                return;
            }

            var filePath = Path.Combine(webRootPath, trainer.ProfileImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            trainer.ProfileImageUrl = null;
            await _context.SaveChangesAsync();
        }
    }
}