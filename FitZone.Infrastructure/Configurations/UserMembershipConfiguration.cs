using FitZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitZone.Infrastructure.Configurations
{
    public class UserMembershipConfiguration : IEntityTypeConfiguration<UserMembership>
    {
        public void Configure(EntityTypeBuilder<UserMembership> builder)
        {
            builder.HasKey(um => um.Id);

            builder.HasOne(um => um.User)
                .WithMany(um => um.UserMemberships)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(um => um.MembershipPlan)
                .WithMany(um => um.UserMemberships)
                .HasForeignKey(um => um.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(um => um.StartDate)
                .IsRequired();

            builder.Property(um => um.EndDate)
                .IsRequired();
        }
    }
}
