using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration
    : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CartId)
            .IsRequired();

        builder.Property(x => x.ProductVariantId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // A variant should appear only once in a cart.
        builder.HasIndex(x => new
        {
            x.CartId,
            x.ProductVariantId
        })
        .IsUnique();

        // CartItem → ProductVariant
        builder.HasOne(x => x.ProductVariant)
            .WithMany()
            .HasForeignKey(x => x.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}