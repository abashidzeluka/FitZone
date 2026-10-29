using FitZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FitZone.Infrastructure.Configurations
{
    public class VisitConfiguration : IEntityTypeConfiguration<Visit>
    {
        public void Configure(EntityTypeBuilder<Visit> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasOne(v => v.User)
                .WithMany(v => v.Visits)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.UserMembership)
                .WithMany()
                .HasForeignKey(v => v.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(v => v.CheckedInAt)
                .IsRequired();
        }
    }
}
