using FitZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitZone.Infrastructure.Configurations
{
    public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
    {
        public void Configure(EntityTypeBuilder<TrainingSession> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Capacity)
                .IsRequired();

            builder.HasOne(s => s.Trainer)
                .WithMany()
                .HasForeignKey(s => s.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
