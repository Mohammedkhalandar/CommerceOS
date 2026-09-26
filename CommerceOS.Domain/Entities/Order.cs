using CommerceOS.Domain.Common;
using CommerceOS.Domain.Enums;

namespace CommerceOS.Domain.Entities;

public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    public Order(
        Guid customerId,
        string shippingFullName,
        string shippingAddressLine1,
        string shippingCity,
        string shippingState,
        string shippingPostalCode,
        string shippingCountry)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID is required.");

        if (string.IsNullOrWhiteSpace(shippingFullName))
            throw new ArgumentException(
                "Shipping name is required.");

        if (string.IsNullOrWhiteSpace(shippingAddressLine1))
            throw new ArgumentException(
                "Shipping address is required.");

        if (string.IsNullOrWhiteSpace(shippingCity))
            throw new ArgumentException(
                "Shipping city is required.");

        if (string.IsNullOrWhiteSpace(shippingState))
            throw new ArgumentException(
                "Shipping state is required.");

        if (string.IsNullOrWhiteSpace(shippingPostalCode))
            throw new ArgumentException(
                "Shipping postal code is required.");

        if (string.IsNullOrWhiteSpace(shippingCountry))
            throw new ArgumentException(
                "Shipping country is required.");

        CustomerId = customerId;

        ShippingFullName = shippingFullName;
        ShippingAddressLine1 = shippingAddressLine1;
        ShippingCity = shippingCity;
        ShippingState = shippingState;
        ShippingPostalCode = shippingPostalCode;
        ShippingCountry = shippingCountry;

        Status = OrderStatus.Pending;
    }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal SubTotal { get; private set; }

    public decimal ShippingCost { get; private set; }

    public decimal Tax { get; private set; }

    public decimal Total { get; private set; }

    public string ShippingFullName { get; private set; } = null!;

    public string ShippingAddressLine1 { get; private set; } = null!;

    public string? ShippingAddressLine2 { get; private set; }

    public string ShippingCity { get; private set; } = null!;

    public string ShippingState { get; private set; } = null!;

    public string ShippingPostalCode { get; private set; } = null!;

    public string ShippingCountry { get; private set; } = null!;

    public IReadOnlyCollection<OrderItem> Items =>
        _items.AsReadOnly();

    public void AddItem(
        Guid productVariantId,
        string productName,
        string sku,
        int quantity,
        decimal unitPrice)
    {
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

        var item = new OrderItem(
            Id,
            productVariantId,
            productName,
            sku,
            quantity,
            unitPrice);

        _items.Add(item);

        RecalculateTotals();

        MarkUpdated();
    }

    public void SetShippingCost(decimal shippingCost)
    {
        if (shippingCost < 0)
            throw new ArgumentException(
                "Shipping cost cannot be negative.");

        ShippingCost = shippingCost;

        RecalculateTotals();

        MarkUpdated();
    }

    public void SetTax(decimal tax)
    {
        if (tax < 0)
            throw new ArgumentException(
                "Tax cannot be negative.");

        Tax = tax;

        RecalculateTotals();

        MarkUpdated();
    }

    public void MarkPaymentPending()
    {
        EnsureStatus(OrderStatus.Pending);

        Status = OrderStatus.PaymentPending;

        MarkUpdated();
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.PaymentPending &&
            Status != OrderStatus.Processing)
        {
            throw new InvalidOperationException(
                "Order cannot be marked as paid.");
        }

        Status = OrderStatus.Paid;

        MarkUpdated();
    }

    public void StartProcessing()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException(
                "Only paid orders can enter processing.");

        Status = OrderStatus.Processing;

        MarkUpdated();
    }

    public void Ship()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException(
                "Only processing orders can be shipped.");

        Status = OrderStatus.Shipped;

        MarkUpdated();
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException(
                "Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;

        MarkUpdated();
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped ||
            Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException(
                "Shipped or delivered orders cannot be cancelled.");
        }

        Status = OrderStatus.Cancelled;

        MarkUpdated();
    }

    private void RecalculateTotals()
    {
        SubTotal = _items.Sum(x => x.GetTotal());

        Total = SubTotal + ShippingCost + Tax;
    }

    private void EnsureStatus(OrderStatus expected)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException(
                $"Order must be {expected}.");
        }
    }
}