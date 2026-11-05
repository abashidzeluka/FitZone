using FitZone.Application.DTOs.Members;
using FitZone.Application.Interfaces;
using FitZone.Domain.Entities;
using FitZone.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitZone.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly FitZoneDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberService(FitZoneDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<MemberDto>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            var memberDtos = new List<MemberDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                memberDtos.Add(new MemberDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }

            return memberDtos;
        }

        public async Task<MemberDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new MemberDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            };
        }

        public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, "Member");

            return new MemberDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = new List<string> { "Member" }
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}