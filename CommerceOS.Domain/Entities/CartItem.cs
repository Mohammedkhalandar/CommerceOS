using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class CartItem : BaseEntity
{
    private CartItem()
    {
    }

    public CartItem(
        Guid cartId,
        Guid productVariantId,
        int quantity,
        decimal unitPrice)
    {
        if (cartId == Guid.Empty)
            throw new ArgumentException("Cart ID is required.");

        if (productVariantId == Guid.Empty)
            throw new ArgumentException(
                "Product variant ID is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.");

        CartId = cartId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid CartId { get; private set; }

    public Guid ProductVariantId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public Cart Cart { get; private set; } = null!;

    public ProductVariant ProductVariant { get; private set; } = null!;

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Quantity += quantity;

        MarkUpdated();
    }

    public void DecreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Quantity -= quantity;

        if (Quantity < 1)
            Quantity = 1;

        MarkUpdated();
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        Quantity = quantity;

        MarkUpdated();
    }

    public decimal GetTotal()
    {
        return UnitPrice * Quantity;
    }
}