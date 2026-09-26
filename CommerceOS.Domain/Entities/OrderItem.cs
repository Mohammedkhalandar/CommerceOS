using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class OrderItem : BaseEntity
{
    private OrderItem()
    {
    }

    public OrderItem(
        Guid orderId,
        Guid productVariantId,
        string productName,
        string sku,
        int quantity,
        decimal unitPrice)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        if (productVariantId == Guid.Empty)
            throw new ArgumentException(
                "Product variant ID is required.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException(
                "Product name is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException(
                "SKU is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.");

        OrderId = orderId;
        ProductVariantId = productVariantId;
        ProductName = productName;
        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid OrderId { get; private set; }

    public Guid ProductVariantId { get; private set; }

    public string ProductName { get; private set; } = null!;

    public string Sku { get; private set; } = null!;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public Order Order { get; private set; } = null!;

    public ProductVariant ProductVariant { get; private set; } = null!;

    public decimal GetTotal()
    {
        return UnitPrice * Quantity;
    }
}