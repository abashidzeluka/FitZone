using System.Collections.Generic;
using System.Threading.Tasks;
using FitZone.Application.DTOs.Visits;

namespace FitZone.Application.Interfaces
{
    public interface IVisitService
    {
        Task<VisitDto> CheckInAsync(int userId);
        Task<IEnumerable<VisitDto>> GetMyVisitsAsync(int userId);
    }
}