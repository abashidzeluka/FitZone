using Microsoft.AspNetCore.Identity;

namespace FitZone.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DateOfBirth { get; set; }

    public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}