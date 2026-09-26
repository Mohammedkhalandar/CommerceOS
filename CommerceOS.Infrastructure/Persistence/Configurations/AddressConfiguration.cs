using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class AddressConfiguration
    : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.AddressLine2)
            .HasMaxLength(250);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsDefault)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // Customer → Address
        builder.HasOne<Customer>()
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CustomerId);
    }
}