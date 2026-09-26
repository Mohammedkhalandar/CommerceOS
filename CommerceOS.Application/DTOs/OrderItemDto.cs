namespace CommerceOS.Application.DTOs.Orders;

public class OrderItemDto
{
    public Guid Id { get; set; }

    public Guid ProductVariantId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total { get; set; }
}