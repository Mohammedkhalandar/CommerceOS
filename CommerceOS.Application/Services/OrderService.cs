using CommerceOS.Application.BackgroundJobs;
using CommerceOS.Application.DTOs.Orders;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductVariantRepository _variantRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IInventoryReservationRepository
        _inventoryReservationRepository;
    private readonly ICommerceTransaction _commerceTransaction;

    private readonly IBackgroundJobQueue _backgroundJobQueue;

    public OrderService(
        IOrderRepository orderRepository,
        IProductVariantRepository variantRepository,
        IInventoryRepository inventoryRepository,
        ICustomerRepository customerRepository,
        IInventoryReservationRepository inventoryReservationRepository,
        ICommerceTransaction commerceTransaction,
        IBackgroundJobQueue backgroundJobQueue)
    {
        _orderRepository = orderRepository;
        _variantRepository = variantRepository;
        _inventoryRepository = inventoryRepository;
        _customerRepository = customerRepository;
        _inventoryReservationRepository =
            inventoryReservationRepository;
        _commerceTransaction = commerceTransaction;

        _backgroundJobQueue = backgroundJobQueue;
    }

    public async Task<OrderDto> CreateAsync(
        CreateOrderDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Order? createdOrder = null;

        await _commerceTransaction.ExecuteAsync(
            async transactionCancellationToken =>
            {
                var customer =
                    await _customerRepository.GetByIdAsync(
                        request.CustomerId,
                        transactionCancellationToken);

                if (customer is null)
                {
                    throw new KeyNotFoundException(
                        "Customer was not found.");
                }

                if (!customer.IsActive)
                {
                    throw new InvalidOperationException(
                        "Customer is inactive.");
                }

                if (request.Items is null ||
                    request.Items.Count == 0)
                {
                    throw new ArgumentException(
                        "Order must contain at least one item.");
                }

                var order = new Order(
                    request.CustomerId,
                    request.ShippingFullName,
                    request.ShippingAddressLine1,
                    request.ShippingCity,
                    request.ShippingState,
                    request.ShippingPostalCode,
                    request.ShippingCountry);

                foreach (var item in request.Items)
                {
                    if (item.ProductVariantId == Guid.Empty)
                    {
                        throw new ArgumentException(
                            "Product variant ID is required.");
                    }

                    if (item.Quantity <= 0)
                    {
                        throw new ArgumentException(
                            "Item quantity must be greater than zero.");
                    }

                    var variant =
                        await _variantRepository.GetByIdAsync(
                            item.ProductVariantId,
                            transactionCancellationToken);

                    if (variant is null)
                    {
                        throw new KeyNotFoundException(
                            $"Product variant '{item.ProductVariantId}' was not found.");
                    }

                    if (!variant.IsActive)
                    {
                        throw new InvalidOperationException(
                            $"Product variant '{variant.Sku}' is inactive.");
                    }

                    var inventory =
                        await _inventoryRepository.GetByVariantIdAsync(
                            variant.Id,
                            transactionCancellationToken);

                    if (inventory is null)
                    {
                        throw new InvalidOperationException(
                            $"Inventory does not exist for SKU '{variant.Sku}'.");
                    }

                    if (!inventory.HasAvailableStock(item.Quantity))
                    {
                        throw new InvalidOperationException(
                            $"Insufficient inventory for SKU '{variant.Sku}'.");
                    }

                    order.AddItem(
                        variant.Id,
                        variant.Product.Name,
                        variant.Sku,
                        item.Quantity,
                        variant.Price);

                    inventory.ReserveStock(item.Quantity);

                    var reservation =
                        new InventoryReservation(
                            inventory.Id,
                            order.Id,
                            item.Quantity,
                            DateTime.UtcNow.AddMinutes(30));

                    await _inventoryReservationRepository.AddAsync(
                        reservation,
                        transactionCancellationToken);
                }

                await _orderRepository.AddAsync(
                    order,
                    transactionCancellationToken);

                createdOrder = order;
            },
            cancellationToken);

        // ==================================================
        // QUEUE BACKGROUND JOB
        // ==================================================

        var orderId = createdOrder!.Id;

        await _backgroundJobQueue.QueueAsync(
            async backgroundCancellationToken =>
            {
                Console.WriteLine(
                    $"Background job started for Order: {orderId}");

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    backgroundCancellationToken);

                Console.WriteLine(
                    $"Background job completed for Order: {orderId}");

                await ValueTask.CompletedTask;
            });

        return MapToDto(createdOrder);
    }

    public async Task<OrderDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(
            id,
            cancellationToken);

        return order is null
            ? null
            : MapToDto(order);
    }

    public async Task<IReadOnlyList<OrderDto>>
        GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
    {
        var orders =
            await _orderRepository.GetByCustomerIdAsync(
                customerId,
                cancellationToken);

        return orders
            .Select(MapToDto)
            .ToList();
    }

    public async Task<bool> MarkPaymentPendingAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(
            id,
            cancellationToken);

        if (order is null)
            return false;

        order.MarkPaymentPending();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> MarkPaidAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(
            id,
            cancellationToken);

        if (order is null)
            return false;

        order.MarkPaid();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> StartProcessingAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(
            id,
            cancellationToken);

        if (order is null)
            return false;

        order.StartProcessing();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> ShipAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(
            id,
            cancellationToken);

        if (order is null)
            return false;

        order.Ship();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> DeliverAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(
            id,
            cancellationToken);

        if (order is null)
            return false;

        order.Deliver();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var order = await GetOrderAsync(
            id,
            cancellationToken);

        if (order is null)
            return false;

        order.Cancel();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private async Task<Order?> GetOrderAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _orderRepository.GetByIdAsync(
            id,
            cancellationToken);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Status = order.Status,
            SubTotal = order.SubTotal,
            ShippingCost = order.ShippingCost,
            Tax = order.Tax,
            Total = order.Total,
            ShippingFullName = order.ShippingFullName,
            ShippingAddressLine1 = order.ShippingAddressLine1,
            ShippingAddressLine2 = order.ShippingAddressLine2,
            ShippingCity = order.ShippingCity,
            ShippingState = order.ShippingState,
            ShippingPostalCode = order.ShippingPostalCode,
            ShippingCountry = order.ShippingCountry,
            CreatedAtUtc = order.CreatedAtUtc,
            UpdatedAtUtc = order.UpdatedAtUtc,

            Items = order.Items
                .Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductVariantId = item.ProductVariantId,
                    ProductName = item.ProductName,
                    Sku = item.Sku,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.GetTotal()
                })
                .ToList()
        };
    }
}