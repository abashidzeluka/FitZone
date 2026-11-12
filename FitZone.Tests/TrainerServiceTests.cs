using FitZone.Application.DTOs.Trainers;
using FitZone.Application.Services;
using FitZone.Domain.Entities;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FitZone.Tests
{
    public class TrainerServiceTests
    {
        private FitZoneDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FitZoneDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new FitZoneDbContext(options);
        }

        [Fact]
        public async Task UpdateTrainerAsync_ByOwnerTrainer_UpdatesProfileSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            var service = new TrainerService(context);

            var trainer = new Trainer
            {
                Id = 1,
                UserId = 10,
                FullName = "Old Name",
                Specialization = "General Fitness"
            };
            context.Trainers.Add(trainer);
            await context.SaveChangesAsync();

            var dto = new UpdateTrainerDto
            {
                FullName = "Alex Mercer",
                Specialization = "Powerlifting & Strength"
            };

            var result = await service.UpdateTrainerAsync(trainerId: 1, dto, currentUserId: 10, isAdmin: false);

            Assert.Equal("Alex Mercer", result.FullName);
            Assert.Equal("Powerlifting & Strength", result.Specialization);
        }

        [Fact]
        public async Task UpdateTrainerAsync_ByAnotherTrainer_ThrowsUnauthorizedAccessException()
        {
            using var context = GetInMemoryDbContext();
            var service = new TrainerService(context);

            var trainer = new Trainer
            {
                Id = 1,
                UserId = 10,
                FullName = "Alex Mercer",
                Specialization = "General Fitness"
            };
            context.Trainers.Add(trainer);
            await context.SaveChangesAsync();

            var dto = new UpdateTrainerDto
            {
                FullName = "Hacked Name",
                Specialization = "Yoga"
            };

            
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.UpdateTrainerAsync(trainerId: 1, dto, currentUserId: 20, isAdmin: false)
            );
        }

        [Fact]
        public async Task UpdateTrainerAsync_ByAdminForAnyTrainer_UpdatesProfileSuccessfully()
        {
            using var context = GetInMemoryDbContext();
            var service = new TrainerService(context);

            var trainer = new Trainer
            {
                Id = 1,
                UserId = 10,
                FullName = "Alex Mercer",
                Specialization = "General Fitness"
            };
            context.Trainers.Add(trainer);
            await context.SaveChangesAsync();

            var dto = new UpdateTrainerDto
            {
                FullName = "Updated By Admin",
                Specialization = "CrossFit"
            };

            var result = await service.UpdateTrainerAsync(trainerId: 1, dto, currentUserId: 99, isAdmin: true);

            Assert.Equal("Updated By Admin", result.FullName);
            Assert.Equal("CrossFit", result.Specialization);
        }

        [Fact]
        public async Task UpdateTrainerAsync_NonExistentTrainer_ThrowsKeyNotFoundException()
        {
            using var context = GetInMemoryDbContext();
            var service = new TrainerService(context);

            var dto = new UpdateTrainerDto { FullName = "Test", Specialization = "Test" };

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.UpdateTrainerAsync(trainerId: 999, dto, currentUserId: 1, isAdmin: true)
            );
        }
    }
}