using FitZone.Application.DTOs.MembershipPlans;
using FitZone.Application.Interfaces;
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

        public Task<MembershipPlanDto> CreateAsync(CreateMembershipPlanDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
