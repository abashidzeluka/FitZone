using FitZone.Application.DTOs.Bookings;
using FitZone.Application.Interfaces;
using FitZone.Domain.Enums;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitZone.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly FitZoneDbContext _context;

        public BookingService(FitZoneDbContext context)
        {
            _context = context;
        }

        public async Task<int> BookSessionAsync(int userId, BookingDto dto)
        {
            var session = await _context.TrainingSessions
                .Include(s => s.Bookings)
                .FirstOrDefaultAsync(s => s.Id == dto.TrainingSessionId);

            if (session == null || session.StartsAt <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Session not found or has already started.");
            }

            var hasActiveMembership = await _context.UserMemberships
                .AnyAsync(m => m.UserId == userId && m.EndDate >= DateTime.UtcNow);

            if (!hasActiveMembership)
            {
                throw new InvalidOperationException("Active membership required to book a session.");
            }

            if (session.Bookings.Any(b => b.UserId == userId && b.Status == BookingStatus.Confirmed))
            {
                throw new InvalidOperationException("You have already booked this session.");
            }

            if (session.Bookings.Count(b => b.Status == BookingStatus.Confirmed) >= session.Capacity)
            {
                throw new InvalidOperationException("Session is fully booked.");
            }

            var booking = new Domain.Entities.Booking
            {
                UserId = userId,
                TrainingSessionId = session.Id,
                Status = BookingStatus.Confirmed
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking.Id;
        }

        public async Task CancelBookingAsync(int bookingId, int userId, bool isAdmin)
        {
            var booking = await _context.Bookings
                .Include(b => b.TrainingSession)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            if (!isAdmin && booking.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this booking.");
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                throw new InvalidOperationException("This booking is already cancelled.");
            }

            if (booking.TrainingSession.StartsAt <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Cannot cancel a booking for a session that has already started.");
            }

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserBookingDto>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .Include(b => b.User)
                .Include(b => b.TrainingSession)
                    .ThenInclude(s => s.Trainer)
                .OrderByDescending(b => b.TrainingSession.StartsAt)
                .Select(b => new UserBookingDto
                {
                    BookingId = b.Id,
                    UserId = b.UserId,
                    UserName = b.User.UserName ?? string.Empty,
                    TrainingSessionId = b.TrainingSessionId,
                    SessionTitle = b.TrainingSession.Title,
                    TrainerName = b.TrainingSession.Trainer.FullName,
                    StartsAt = b.TrainingSession.StartsAt,
                    EndsAt = b.TrainingSession.EndsAt,
                    Status = b.Status.ToString()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<UserBookingDto>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.TrainingSession)
                    .ThenInclude(s => s.Trainer)
                .OrderByDescending(b => b.TrainingSession.StartsAt)
                .Select(b => new UserBookingDto
                {
                    BookingId = b.Id,
                    UserId = b.UserId,
                    UserName = b.User.UserName ?? string.Empty,
                    TrainingSessionId = b.TrainingSessionId,
                    SessionTitle = b.TrainingSession.Title,
                    TrainerName = b.TrainingSession.Trainer.FullName,
                    StartsAt = b.TrainingSession.StartsAt,
                    EndsAt = b.TrainingSession.EndsAt,
                    Status = b.Status.ToString()
                })
                .ToListAsync();
        }
    }
}