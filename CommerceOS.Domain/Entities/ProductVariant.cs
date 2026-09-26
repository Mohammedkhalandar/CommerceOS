using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class ProductVariant : BaseEntity
{
    private ProductVariant()
    {
    }

    public ProductVariant(
        Guid productId,
        string sku,
        decimal price,
        string? color = null,
        string? size = null)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID is required.");

        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU is required.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        ProductId = productId;
        Sku = sku;
        Price = price;
        Color = color;
        Size = size;
    }

    public Guid ProductId { get; private set; }

    public string Sku { get; private set; } = null!;

    public decimal Price { get; private set; }

    public string? Color { get; private set; }

    public string? Size { get; private set; }

    public bool IsActive { get; private set; } = true;

    public Product Product { get; private set; } = null!;

    public Inventory Inventory { get; private set; } = null!;

    public void UpdateDetails(
        string sku,
        decimal price,
        string? color,
        string? size)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU is required.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        Sku = sku;
        Price = price;
        Color = color;
        Size = size;

        MarkUpdated();
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative.");

        Price = newPrice;
        MarkUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }
}