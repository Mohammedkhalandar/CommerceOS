namespace CommerceOS.Application.DTOs;

public class InventoryReservationDto
{
    public Guid Id { get; set; }

    public Guid InventoryId { get; set; }

    public Guid OrderId { get; set; }

    public int Quantity { get; set; }

    public DateTime ExpiresAtUtc { get; set; }

    public bool IsReleased { get; set; }

    public bool IsConfirmed { get; set; }

    public bool IsExpired { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}