using FitZone.Domain.Enums;
using System.Collections.Generic;
using System;

namespace FitZone.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; }
        public ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}