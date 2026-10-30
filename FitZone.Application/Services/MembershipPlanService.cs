using FitZone.Application.DTOs.MembershipPlans;
using FitZone.Application.Interfaces;
using FitZone.Domain.Entities;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Application.Services
{
    public class MembershipPlanService : IMembershipPlanService
    {
        private readonly FitZoneDbContext _context;
        
        public MembershipPlanService(FitZoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MembershipPlanDto>> GetAllAsync()
        {
            return await _context.MembershipPlans
            .Select(p => new MembershipPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                VisitLimit = p.VisitLimit,
                Price = p.Price,
                DurationDays = p.DurationDays
            })
            .ToListAsync();
        }

        public async Task<MembershipPlanDto?> GetByIdAsync(int id)
        {
            var plan = await _context.MembershipPlans.FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
            {
                return null;
            }

            return new MembershipPlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                VisitLimit = plan.VisitLimit,
                Price = plan.Price,
                DurationDays = plan.DurationDays
            };
        }

        public async Task<MembershipPlanDto> CreateAsync(CreateMembershipPlanDto dto)
        {
            var plan = new MembershipPlan
            {
                Name = dto.Name,
                VisitLimit = dto.VisitLimit,
                Price = dto.Price,
                DurationDays = dto.DurationDays
            };

            _context.MembershipPlans.Add(plan);
            await _context.SaveChangesAsync();

            return new MembershipPlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                VisitLimit = plan.VisitLimit,
                Price = plan.Price,
                DurationDays = plan.DurationDays
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var plan = await _context.MembershipPlans.FindAsync(id);

            if(plan == null)
            {
                return false;
            }

            _context.MembershipPlans.Remove(plan);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
