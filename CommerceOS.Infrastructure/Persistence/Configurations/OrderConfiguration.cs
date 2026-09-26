using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.SubTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ShippingCost)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Tax)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ShippingFullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ShippingAddressLine1)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(x => x.ShippingAddressLine2)
            .HasMaxLength(250);

        builder.Property(x => x.ShippingCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingState)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ShippingPostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ShippingCountry)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => x.Status);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}