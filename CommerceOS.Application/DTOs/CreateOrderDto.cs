namespace CommerceOS.Application.DTOs.Orders;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }

    public string ShippingFullName { get; set; } = null!;

    public string ShippingAddressLine1 { get; set; } = null!;

    public string? ShippingAddressLine2 { get; set; }

    public string ShippingCity { get; set; } = null!;

    public string ShippingState { get; set; } = null!;

    public string ShippingPostalCode { get; set; } = null!;

    public string ShippingCountry { get; set; } = null!;

    public List<CreateOrderItemDto> Items { get; set; } = [];
}

public class CreateOrderItemDto
{
    public Guid ProductVariantId { get; set; }

    public int Quantity { get; set; }
}