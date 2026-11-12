using FitZone.Application.DTOs.Bookings;
using FitZone.Application.Services;
using FitZone.Domain.Entities;
using FitZone.Domain.Enums;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FitZone.Tests
{
    public class BookingServiceTests
    {
        private FitZoneDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FitZoneDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new FitZoneDbContext(options);
        }

        [Fact]
        public async Task BookSessionAsync_WithActiveMembershipAndAvailableSpots_CreatesConfirmedBooking()
        {
            using var context = GetInMemoryDbContext();
            var service = new BookingService(context);

            var user = new ApplicationUser { Id = 1, UserName = "testuser", Email = "test@test.com" };
            var trainer = new Trainer { Id = 1, FullName = "John Doe", Specialization = "CrossFit" };
            var session = new TrainingSession
            {
                Id = 1,
                Title = "Morning HIIT",
                TrainerId = 1,
                Capacity = 10,
                StartsAt = DateTime.UtcNow.AddDays(1),
                EndsAt = DateTime.UtcNow.AddDays(1).AddHours(1)
            };
            var membership = new UserMembership
            {
                Id = 1,
                UserId = 1,
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(25)
            };

            context.Users.Add(user);
            context.Trainers.Add(trainer);
            context.TrainingSessions.Add(session);
            context.UserMemberships.Add(membership);
            await context.SaveChangesAsync();

            var dto = new BookingDto { TrainingSessionId = 1 };

            var bookingId = await service.BookSessionAsync(userId: 1, dto);

            var savedBooking = await context.Bookings.FindAsync(bookingId);
            Assert.NotNull(savedBooking);
            Assert.Equal(BookingStatus.Confirmed, savedBooking.Status);
            Assert.Equal(1, savedBooking.UserId);
        }

        [Fact]
        public async Task BookSessionAsync_WithoutActiveMembership_ThrowsInvalidOperationException()
        {
            using var context = GetInMemoryDbContext();
            var service = new BookingService(context);

            var session = new TrainingSession
            {
                Id = 1,
                Title = "Evening Yoga",
                TrainerId = 1,
                Capacity = 10,
                StartsAt = DateTime.UtcNow.AddDays(1),
                EndsAt = DateTime.UtcNow.AddDays(1).AddHours(1)
            };

            context.TrainingSessions.Add(session);
            await context.SaveChangesAsync();

            var dto = new BookingDto { TrainingSessionId = 1 };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.BookSessionAsync(userId: 1, dto)
            );

            Assert.Equal("Active membership required to book a session.", exception.Message);
        }

        [Fact]
        public async Task BookSessionAsync_WhenSessionIsFull_ThrowsInvalidOperationException()
        {
            using var context = GetInMemoryDbContext();
            var service = new BookingService(context);

            var session = new TrainingSession
            {
                Id = 1,
                Title = "Boxing",
                TrainerId = 1,
                Capacity = 1,
                StartsAt = DateTime.UtcNow.AddDays(1),
                EndsAt = DateTime.UtcNow.AddDays(1).AddHours(1)
            };

            context.Bookings.Add(new Booking { Id = 1, UserId = 99, TrainingSessionId = 1, Status = BookingStatus.Confirmed });
            context.UserMemberships.Add(new UserMembership { Id = 1, UserId = 1, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(10) });
            context.TrainingSessions.Add(session);
            await context.SaveChangesAsync();

            var dto = new BookingDto { TrainingSessionId = 1 };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.BookSessionAsync(userId: 1, dto)
            );

            Assert.Equal("Session is fully booked.", exception.Message);
        }

        [Fact]
        public async Task CancelBookingAsync_ByAnotherNonAdminUser_ThrowsUnauthorizedAccessException()
        {
            using var context = GetInMemoryDbContext();
            var service = new BookingService(context);

            var session = new TrainingSession
            {
                Id = 1,
                Title = "Pilates",
                TrainerId = 1,
                Capacity = 10,
                StartsAt = DateTime.UtcNow.AddDays(1),
                EndsAt = DateTime.UtcNow.AddDays(1).AddHours(1)
            };
            var booking = new Booking { Id = 10, UserId = 1, TrainingSessionId = 1, Status = BookingStatus.Confirmed };

            context.TrainingSessions.Add(session);
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => service.CancelBookingAsync(bookingId: 10, userId: 2, isAdmin: false)
            );
        }

        [Fact]
        public async Task CancelBookingAsync_ByOwner_CancelsBookingAndFreesSpot()
        {
            using var context = GetInMemoryDbContext();
            var service = new BookingService(context);

            var session = new TrainingSession
            {
                Id = 1,
                Title = "Cardio",
                TrainerId = 1,
                Capacity = 1,
                StartsAt = DateTime.UtcNow.AddDays(1),
                EndsAt = DateTime.UtcNow.AddDays(1).AddHours(1)
            };
            var booking = new Booking { Id = 10, UserId = 1, TrainingSessionId = 1, Status = BookingStatus.Confirmed };

            context.TrainingSessions.Add(session);
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();

            await service.CancelBookingAsync(bookingId: 10, userId: 1, isAdmin: false);

            var updatedBooking = await context.Bookings.FindAsync(10);
            Assert.Equal(BookingStatus.Cancelled, updatedBooking!.Status);
        }
    }
}