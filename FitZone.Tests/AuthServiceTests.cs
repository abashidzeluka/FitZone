using FitZone.Application.DTOs.Auth;
using FitZone.Application.Services;
using FitZone.Domain.Entities;
using FitZone.Domain.Enums;
using FitZone.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FitZone.Tests
{
    public class AuthServiceTests
    {
        private FitZoneDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FitZoneDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new FitZoneDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private Mock<RoleManager<IdentityRole<int>>> GetMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole<int>>>();
            return new Mock<RoleManager<IdentityRole<int>>>(
                store.Object, null!, null!, null!, null!);
        }

        private IConfiguration GetMockConfiguration()
        {
            var settings = new Dictionary<string, string?>
            {
                {"Jwt:Key", "SuperSecretTestKeyThatIsAtLeast32CharactersLong!"},
                {"Jwt:Issuer", "FitZoneTest"},
                {"Jwt:Audience", "FitZoneTestUsers"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public async Task AssignRoleAsync_WhenTrainerRoleAssigned_CreatesTrainerEntityInDatabase()
        {
            using var context = GetInMemoryDbContext();
            var userManagerMock = GetMockUserManager();
            var roleManagerMock = GetMockRoleManager();
            var config = GetMockConfiguration();

            var user = new ApplicationUser { Id = 5, UserName = "coach_mike", Email = "mike@fitzone.com" };

            userManagerMock.Setup(x => x.FindByIdAsync("5")).ReturnsAsync(user);
            roleManagerMock.Setup(x => x.RoleExistsAsync("Trainer")).ReturnsAsync(true);
            userManagerMock.Setup(x => x.IsInRoleAsync(user, "Trainer")).ReturnsAsync(false);
            userManagerMock.Setup(x => x.AddToRoleAsync(user, "Trainer")).ReturnsAsync(IdentityResult.Success);

            var authService = new AuthService(userManagerMock.Object, roleManagerMock.Object, config, context);
            var dto = new AssignRoleDto { UserId = 5, Role = Role.Trainer };

            var result = await authService.AssignRoleAsync(dto);

            Assert.True(result);
            var createdTrainer = await context.Trainers.FirstOrDefaultAsync(t => t.UserId == 5);
            Assert.NotNull(createdTrainer);
            Assert.Equal("coach_mike", createdTrainer.FullName);
            Assert.Equal("General Fitness", createdTrainer.Specialization);
        }

        [Fact]
        public async Task RemoveRoleAsync_WhenTrainerHasNoSessions_RemovesTrainerEntityFromDatabase()
        {
            using var context = GetInMemoryDbContext();
            var userManagerMock = GetMockUserManager();
            var roleManagerMock = GetMockRoleManager();
            var config = GetMockConfiguration();

            var user = new ApplicationUser { Id = 5, UserName = "coach_mike", Email = "mike@fitzone.com" };
            var trainer = new Trainer { Id = 1, UserId = 5, FullName = "coach_mike", Specialization = "General Fitness" };

            context.Trainers.Add(trainer);
            await context.SaveChangesAsync();

            userManagerMock.Setup(x => x.FindByIdAsync("5")).ReturnsAsync(user);
            userManagerMock.Setup(x => x.IsInRoleAsync(user, "Trainer")).ReturnsAsync(true);
            userManagerMock.Setup(x => x.RemoveFromRoleAsync(user, "Trainer")).ReturnsAsync(IdentityResult.Success);

            var authService = new AuthService(userManagerMock.Object, roleManagerMock.Object, config, context);
            var dto = new RemoveRoleDto { UserId = 5, Role = Role.Trainer };

            var result = await authService.RemoveRoleAsync(dto);

            Assert.True(result);
            var deletedTrainer = await context.Trainers.FirstOrDefaultAsync(t => t.UserId == 5);
            Assert.Null(deletedTrainer);
        }
    }
}