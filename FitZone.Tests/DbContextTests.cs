using FitZone.Domain.Entities;
using FitZone.Domain.Enums;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FitZone.Tests
{
    public class DbContextTests
    {
        private FitZoneDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FitZoneDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new FitZoneDbContext(options);
        }

        [Fact]
        public async Task DbContext_CanSaveAndLoadCompleteBookingGraph()
        {
            using var context = GetInMemoryDbContext();

            var member = new ApplicationUser { Id = 1, UserName = "member1", Email = "member@fitzone.com" };
            var trainerUser = new ApplicationUser { Id = 2, UserName = "trainer1", Email = "trainer@fitzone.com" };
            var trainer = new Trainer
            {
                Id = 1,
                UserId = trainerUser.Id,
                FullName = "Giorgi K.",
                Specialization = "Strength & Conditioning"
            };
            var session = new TrainingSession
            {
                Id = 1,
                Title = "Full Body Workout",
                TrainerId = trainer.Id,
                Capacity = 15,
                StartsAt = DateTime.UtcNow.AddDays(2),
                EndsAt = DateTime.UtcNow.AddDays(2).AddHours(1)
            };
            var booking = new Booking
            {
                Id = 1,
                UserId = member.Id,
                TrainingSessionId = session.Id,
                Status = BookingStatus.Confirmed
            };

            context.Users.AddRange(member, trainerUser);
            context.Trainers.Add(trainer);
            context.TrainingSessions.Add(session);
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            var loadedBooking = await context.Bookings
                .Include(b => b.User)
                .Include(b => b.TrainingSession)
                    .ThenInclude(s => s.Trainer)
                .FirstOrDefaultAsync(b => b.Id == 1);

            Assert.NotNull(loadedBooking);
            Assert.Equal("member1", loadedBooking.User.UserName);
            Assert.Equal("Full Body Workout", loadedBooking.TrainingSession.Title);
            Assert.Equal("Giorgi K.", loadedBooking.TrainingSession.Trainer.FullName);
        }

        [Fact]
        public async Task DbContext_UserMembershipAndPlanRelationship_WorksCorrectly()
        {
            using var context = GetInMemoryDbContext();

            var user = new ApplicationUser { Id = 1, UserName = "alex", Email = "alex@fitzone.com" };
            var plan = new MembershipPlan
            {
                Id = 1,
                Name = "Gold Monthly",
                Price = 120m,
                DurationDays = 30,
                VisitLimit = 12
            };
            var membership = new UserMembership
            {
                Id = 1,
                UserId = user.Id,
                MembershipPlan = plan,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            context.Users.Add(user);
            context.MembershipPlans.Add(plan);
            context.UserMemberships.Add(membership);
            await context.SaveChangesAsync();

            var activeMemberships = await context.UserMemberships
                .Include(um => um.MembershipPlan)
                .Where(um => um.UserId == user.Id && um.EndDate > DateTime.UtcNow)
                .ToListAsync();
            Assert.Single(activeMemberships);
            Assert.Equal("Gold Monthly", activeMemberships[0].MembershipPlan.Name);
            Assert.Equal(120m, activeMemberships[0].MembershipPlan.Price);
            Assert.Equal(12, activeMemberships[0].MembershipPlan.VisitLimit);
        }

        [Fact]
        public async Task DbContext_TrainerSessionsCollection_TracksAllAssignedSessions()
        {
            using var context = GetInMemoryDbContext();

            var trainer = new Trainer
            {
                Id = 1,
                FullName = "Nika M.",
                Specialization = "Boxing"
            };

            context.Trainers.Add(trainer);
            context.TrainingSessions.AddRange(
                new TrainingSession { Id = 1, Title = "Morning Boxing", TrainerId = 1, Capacity = 10, StartsAt = DateTime.UtcNow.AddDays(1), EndsAt = DateTime.UtcNow.AddDays(1).AddHours(1) },
                new TrainingSession { Id = 2, Title = "Evening Sparring", TrainerId = 1, Capacity = 8, StartsAt = DateTime.UtcNow.AddDays(2), EndsAt = DateTime.UtcNow.AddDays(2).AddHours(1) }
            );
            await context.SaveChangesAsync();

            var trainerWithSessions = await context.Trainers
                .Include(t => t.TrainingSessions)
                .FirstOrDefaultAsync(t => t.Id == 1);

            Assert.NotNull(trainerWithSessions);
            Assert.Equal(2, trainerWithSessions.TrainingSessions.Count);
        }

        [Fact]
        public async Task DbContext_VisitTracking_SavesAndQueriesUserCheckIns()
        {
            using var context = GetInMemoryDbContext();

            var user = new ApplicationUser { Id = 1, UserName = "gym_goer", Email = "goer@fitzone.com" };
            var plan = new MembershipPlan { Id = 1, Name = "Standard", Price = 80m, DurationDays = 30 };
            var membership = new UserMembership
            {
                Id = 1,
                UserId = 1,
                MembershipPlan = plan,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(25)
            };

            var visit1 = new Visit
            {
                Id = 1,
                UserId = 1,
                MembershipId = 1,
                UserMembership = membership,
                CheckedInAt = DateTime.UtcNow.AddDays(-2)
            };
            var visit2 = new Visit
            {
                Id = 2,
                UserId = 1,
                MembershipId = 1,
                UserMembership = membership,
                CheckedInAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            context.MembershipPlans.Add(plan);
            context.UserMemberships.Add(membership);
            context.Visits.AddRange(visit1, visit2);
            await context.SaveChangesAsync();

            var userVisits = await context.Visits
                .Include(v => v.UserMembership)
                .Where(v => v.UserId == 1)
                .OrderByDescending(v => v.CheckedInAt)
                .ToListAsync();

            Assert.Equal(2, userVisits.Count);
            Assert.Equal(2, userVisits.First().Id);
            Assert.Equal(1, userVisits.First().MembershipId);
        }
    }
}