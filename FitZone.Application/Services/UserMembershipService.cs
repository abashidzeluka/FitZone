using FitZone.Application.DTOs.UserMemberships;
using FitZone.Application.Interfaces;
using FitZone.Domain.Entities;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Application.Services
{
    public class UserMembershipService : IUserMembershipService
    {
        private readonly FitZoneDbContext _context;

        public UserMembershipService(FitZoneDbContext context)
        {
            _context = context;
        }

        public async Task<UserMembershipDto> CreateAsync(CreateUserMembershipDto dto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);

            if(!userExists)
            {
                throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");
            }

            var plan = await _context.MembershipPlans.FindAsync(dto.PlanId);

            if(plan == null)
            {
                throw new KeyNotFoundException();
            }

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(plan.DurationDays);

            var userMembership = new UserMembership
            {
                UserId = dto.UserId!.Value,
                PlanId = dto.PlanId,
                StartDate = startDate,
                EndDate = endDate,
                RemainingVisits = plan.VisitLimit
            };

            _context.UserMemberships.Add(userMembership);
            await _context.SaveChangesAsync();

            return new UserMembershipDto
            {
                Id = userMembership.Id,
                UserId = userMembership.UserId,
                PlanId = userMembership.PlanId,
                PlanName = plan.Name,
                StartDate = userMembership.StartDate,
                EndDate = userMembership.EndDate,
                RemainingVisits = userMembership.RemainingVisits,
                IsActive = userMembership.EndDate > DateTime.UtcNow
            };
        }

        public async Task<UserMembershipDto?> GetByIdAsync(int id)
        {
            var membership = await _context.UserMemberships
                .Include(um => um.MembershipPlan)
                .FirstOrDefaultAsync(um => um.Id == id);

            if(membership == null)
            {
                return null;
            }

            return new UserMembershipDto
            {
                Id = membership.Id,
                UserId = membership.UserId,
                PlanId = membership.PlanId,
                PlanName = membership.MembershipPlan.Name,
                StartDate = membership.StartDate,
                EndDate = membership.EndDate,
                RemainingVisits = membership.RemainingVisits,
                IsActive = membership.EndDate > DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<UserMembershipDto>> GetByUserIdAsync(int userId)
        {
            return await _context.UserMemberships
                .Where(um => um.UserId == userId)
                .Include(um => um.MembershipPlan)
                .Select(um => new UserMembershipDto
                {
                    Id = um.Id,
                    UserId = um.UserId,
                    PlanId = um.PlanId,
                    PlanName = um.MembershipPlan.Name,
                    StartDate = um.StartDate,
                    EndDate = um.EndDate,
                    RemainingVisits = um.RemainingVisits,
                    IsActive = um.EndDate > DateTime.UtcNow
                })
                .ToListAsync();
        }
        public async Task<bool> CancelAsync(int id)
        {
            var membership = await _context.UserMemberships.FindAsync(id);

            if(membership == null)
            {
                return false;
            }

            membership.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }      
    }
}
