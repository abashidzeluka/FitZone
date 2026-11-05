using FitZone.Application.DTOs.Bookings;
using FitZone.Application.Interfaces;
using FitZone.Domain.Enums;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
