using CommerceOS.Domain.Enums;

namespace CommerceOS.Application.DTOs.Orders;

public class OrderDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public OrderStatus Status { get; set; }

    public decimal SubTotal { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    public string ShippingFullName { get; set; } = null!;

    public string ShippingAddressLine1 { get; set; } = null!;

    public string? ShippingAddressLine2 { get; set; }

    public string ShippingCity { get; set; } = null!;

    public string ShippingState { get; set; } = null!;

    public string ShippingPostalCode { get; set; } = null!;

    public string ShippingCountry { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public List<OrderItemDto> Items { get; set; } = [];
}