using FitZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitZone.Infrastructure.Data;

public class FitZoneDbContext : DbContext
{
    public FitZoneDbContext(DbContextOptions<FitZoneDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Trainer> Trainers => Set<Trainer>();
    public DbSet<MembershipPlan> MembershipPlans => Set<MembershipPlan>();
    public DbSet<UserMembership> UserMemberships => Set<UserMembership>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Visit> Visits => Set<Visit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitZoneDbContext).Assembly);
    }
}