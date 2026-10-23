using FitZone.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitZone.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(n => n.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(n => n.PasswordHash)
            .IsRequired();

        builder.HasIndex(n => n.Email)
            .IsUnique();

        builder.HasIndex(n => n.UserName)
            .IsUnique();
    }
}