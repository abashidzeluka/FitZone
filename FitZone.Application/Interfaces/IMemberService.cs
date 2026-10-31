using FitZone.Application.DTOs.Members;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitZone.Application.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberDto>> GetAllAsync();
        Task<MemberDto?> GetByIdAsync(int id);
        Task<MemberDto> CreateAsync(CreateMemberDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
