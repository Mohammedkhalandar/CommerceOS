using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommerceOS.Infrastructure.Persistence.Configurations;

public class InventoryReservationConfiguration
    : IEntityTypeConfiguration<InventoryReservation>
{
    public void Configure(
        EntityTypeBuilder<InventoryReservation> builder)
    {
        builder.ToTable("InventoryReservations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InventoryId)
            .IsRequired();

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.Property(x => x.IsReleased)
            .IsRequired();

        builder.Property(x => x.IsConfirmed)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);

        builder.HasIndex(x => new
        {
            x.InventoryId,
            x.OrderId
        });

        builder.HasIndex(x => x.ExpiresAtUtc);

        builder.HasOne(x => x.Inventory)
            .WithMany()
            .HasForeignKey(x => x.InventoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}