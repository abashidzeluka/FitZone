using FitZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitZone.Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
            public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasOne(b => b.User)
                .WithMany(b => b.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.TrainingSession)
                .WithMany(b => b.Bookings)
                .HasForeignKey(b => b.TrainingSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => new { b.UserId, b.TrainingSessionId })
                .IsUnique();
        }
    }
}
