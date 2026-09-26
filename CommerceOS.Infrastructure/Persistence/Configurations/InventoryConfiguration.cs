using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class InventoryConfiguration
    : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductVariantId)
            .IsRequired();

        builder.Property(x => x.AvailableQuantity)
            .IsRequired();

        builder.Property(x => x.ReservedQuantity)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.Property(x => x.Version)
            .IsConcurrencyToken()
            .IsRequired();

        // One inventory record per product variant.
        builder.HasIndex(x => x.ProductVariantId)
            .IsUnique();

        // ProductVariant → Inventory
        builder.HasOne(x => x.ProductVariant)
            .WithOne(x => x.Inventory)
            .HasForeignKey<Inventory>(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}