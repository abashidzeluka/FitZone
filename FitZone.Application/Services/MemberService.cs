using FitZone.Application.DTOs.Members;
using FitZone.Application.Interfaces;
using FitZone.Domain.Entities;
using FitZone.Domain.Enums;
using FitZone.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly FitZoneDbContext _context;

        public MemberService(FitZoneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MemberDto>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();

            return users.Select(u => new MemberDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role.ToString()
            });
        }

        public async Task<MemberDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return null;
            }

            return new MemberDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
        public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
        {
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                Role = Role.User
            };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            
            return new MemberDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
