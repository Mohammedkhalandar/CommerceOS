using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.ProductVariantId)
            .IsRequired();

        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => x.OrderId);

        builder.HasIndex(x => x.ProductVariantId);

        builder.HasOne(x => x.ProductVariant)
            .WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}