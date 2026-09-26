using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class Inventory : BaseEntity
{
    private Inventory()
    {
    }

    public Inventory(
        Guid productVariantId,
        int availableQuantity)
    {
        if (productVariantId == Guid.Empty)
            throw new ArgumentException("Product variant ID is required.");

        if (availableQuantity < 0)
            throw new ArgumentException(
                "Available quantity cannot be negative.");

        ProductVariantId = productVariantId;
        AvailableQuantity = availableQuantity;
    }

    public Guid ProductVariantId { get; private set; }

    public int AvailableQuantity { get; private set; }

    public int ReservedQuantity { get; private set; }

    public int TotalQuantity =>
        AvailableQuantity + ReservedQuantity;

    public Guid Version { get; private set; } = Guid.NewGuid();

    public ProductVariant ProductVariant { get; private set; } = null!;

    public bool HasAvailableStock(int quantity)
    {
        return quantity > 0 &&
               AvailableQuantity >= quantity;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        AvailableQuantity += quantity;
        TouchVersion();
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException(
                "Insufficient inventory.");

        AvailableQuantity -= quantity;
        ReservedQuantity += quantity;
        TouchVersion();
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException(
                "Cannot release more stock than reserved.");

        ReservedQuantity -= quantity;
        AvailableQuantity += quantity;
        TouchVersion();
    }

    public void ConfirmReservation(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException(
                "Cannot confirm more stock than reserved.");

        ReservedQuantity -= quantity;
        TouchVersion();
    }

    private void TouchVersion()
    {
        Version = Guid.NewGuid();
        MarkUpdated();
    }
}