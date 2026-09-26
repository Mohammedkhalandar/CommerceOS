namespace CommerceOS.Application.DTOs;

public class InventoryDto
{
    public Guid Id { get; set; }

    public Guid ProductVariantId { get; set; }

    public int AvailableQuantity { get; set; }

    public int ReservedQuantity { get; set; }

    public int TotalQuantity { get; set; }

    public Guid Version { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}