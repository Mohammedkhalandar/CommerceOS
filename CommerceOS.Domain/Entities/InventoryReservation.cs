using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class InventoryReservation : BaseEntity
{
    private InventoryReservation()
    {
    }

    public InventoryReservation(
        Guid inventoryId,
        Guid orderId,
        int quantity,
        DateTime expiresAtUtc)
    {
        if (inventoryId == Guid.Empty)
            throw new ArgumentException(
                "Inventory ID is required.");

        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (expiresAtUtc <= DateTime.UtcNow)
            throw new ArgumentException(
                "Expiration must be in the future.");

        InventoryId = inventoryId;
        OrderId = orderId;
        Quantity = quantity;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid InventoryId { get; private set; }

    public Guid OrderId { get; private set; }

    public int Quantity { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public bool IsReleased { get; private set; }

    public bool IsConfirmed { get; private set; }

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAtUtc;

    public Inventory Inventory { get; private set; } = null!;

    public void Confirm()
    {
        if (IsReleased)
            throw new InvalidOperationException(
                "Reservation has already been released.");

        if (IsExpired)
            throw new InvalidOperationException(
                "Reservation has expired.");

        IsConfirmed = true;

        MarkUpdated();
    }

    public void Release()
    {
        if (IsConfirmed)
            throw new InvalidOperationException(
                "Confirmed reservation cannot be released.");

        IsReleased = true;

        MarkUpdated();
    }
}