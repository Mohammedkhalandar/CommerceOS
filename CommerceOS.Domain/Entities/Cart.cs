using CommerceOS.Domain.Common;

namespace CommerceOS.Domain.Entities;

public class Cart : BaseEntity
{
    private readonly List<CartItem> _items = [];

    private Cart()
    {
    }

    public Cart(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID is required.");

        CustomerId = customerId;
    }

    public Guid CustomerId { get; private set; }

    public IReadOnlyCollection<CartItem> Items =>
        _items.AsReadOnly();

    public void AddItem(
        Guid productVariantId,
        int quantity,
        decimal unitPrice)
    {
        if (productVariantId == Guid.Empty)
            throw new ArgumentException("Product variant ID is required.");

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.");

        var existingItem = _items.FirstOrDefault(
            x => x.ProductVariantId == productVariantId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            _items.Add(
                new CartItem(
                    Id,
                    productVariantId,
                    quantity,
                    unitPrice));
        }

        MarkUpdated();
    }

    public void RemoveItem(Guid productVariantId)
    {
        var item = _items.FirstOrDefault(
            x => x.ProductVariantId == productVariantId);

        if (item is null)
            return;

        _items.Remove(item);

        MarkUpdated();
    }

    public void Clear()
    {
        _items.Clear();
        MarkUpdated();
    }

    public decimal CalculateTotal()
    {
        return _items.Sum(x => x.GetTotal());
    }
}