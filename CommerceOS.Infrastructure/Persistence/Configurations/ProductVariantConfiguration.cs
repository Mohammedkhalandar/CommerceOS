using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration
    : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Color)
            .HasMaxLength(100);

        builder.Property(x => x.Size)
            .HasMaxLength(50);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // SKU must be unique across the catalog.
        builder.HasIndex(x => x.Sku)
            .IsUnique();

        // Product → ProductVariant
        builder.HasOne(x => x.Product)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductVariant → Inventory
        builder.HasOne(x => x.Inventory)
            .WithOne(x => x.ProductVariant)
            .HasForeignKey<Inventory>(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}