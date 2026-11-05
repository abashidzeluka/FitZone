using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitZone.Application.DTOs.Visits;
using FitZone.Application.Interfaces;
using FitZone.Domain.Entities;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FitZone.Application.Services
{
    public class VisitService : IVisitService
    {
        private readonly FitZoneDbContext _context;

        public VisitService(FitZoneDbContext context)
        {
            _context = context;
        }

        public async Task<VisitDto> CheckInAsync(int userId)
        {
            var now = DateTime.UtcNow;

            var activeMembership = await _context.UserMemberships
                .Include(m => m.MembershipPlan)
                .Where(m => m.UserId == userId && m.EndDate >= now)
                .Where(m => !m.RemainingVisits.HasValue || m.RemainingVisits.Value > 0)
                .OrderByDescending(m => m.EndDate)
                .FirstOrDefaultAsync();

            if (activeMembership == null)
            {
                throw new InvalidOperationException("Check-in failed: No active membership found or visits limit reached.");
            }

            if (activeMembership.RemainingVisits.HasValue)
            {
                activeMembership.RemainingVisits--;
            }

            var visit = new Visit
            {
                UserId = userId,
                MembershipId = activeMembership.Id,
                CheckedInAt = now
            };

            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();

            return new VisitDto
            {
                Id = visit.Id,
                CheckedInAt = visit.CheckedInAt,
                PlanName = activeMembership.MembershipPlan != null ? activeMembership.MembershipPlan.Name : string.Empty,
                RemainingVisits = activeMembership.RemainingVisits
            };
        }

        public async Task<IEnumerable<VisitDto>> GetMyVisitsAsync(int userId)
        {
            return await _context.Visits
                .AsNoTracking()
                .Include(v => v.UserMembership)
                    .ThenInclude(m => m.MembershipPlan)
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CheckedInAt)
                .Select(v => new VisitDto
                {
                    Id = v.Id,
                    CheckedInAt = v.CheckedInAt,
                    PlanName = v.UserMembership != null && v.UserMembership.MembershipPlan != null
                        ? v.UserMembership.MembershipPlan.Name
                        : string.Empty,
                    RemainingVisits = v.UserMembership != null ? v.UserMembership.RemainingVisits : null
                })
                .ToListAsync();
        }
    }
}