using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // ======================================================
        // TABLE
        // ======================================================

        builder.ToTable("Users");

        // ======================================================
        // PRIMARY KEY
        // ======================================================

        builder.HasKey(x => x.Id);

        // ======================================================
        // EMAIL
        // ======================================================

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        // ======================================================
        // PASSWORD
        // ======================================================

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        // ======================================================
        // ROLE
        // ======================================================

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(50);

        // ======================================================
        // ACTIVE STATUS
        // ======================================================

        builder.Property(x => x.IsActive)
            .IsRequired();

        // ======================================================
        // DATES
        // ======================================================

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired(false);
    }
}