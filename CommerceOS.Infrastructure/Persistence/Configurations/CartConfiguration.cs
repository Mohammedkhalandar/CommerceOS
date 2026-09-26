using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        // One active cart per customer.
        builder.HasIndex(x => x.CustomerId)
            .IsUnique();

        // Customer → Cart
        builder.HasOne<Customer>()
            .WithOne()
            .HasForeignKey<Cart>(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cart → CartItems
        builder.HasMany(x => x.Items)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}