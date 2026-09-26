using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryReservationRepository
        _inventoryReservationRepository;
    private readonly ICommerceTransaction _commerceTransaction;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IInventoryReservationRepository inventoryReservationRepository,
        ICommerceTransaction commerceTransaction)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _inventoryReservationRepository =
            inventoryReservationRepository;
        _commerceTransaction = commerceTransaction;
    }

    public async Task<PaymentDto> CreateAsync(
        Guid orderId,
        decimal amount,
        string provider,
        CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException(
                "Order ID is required.");

        if (amount <= 0)
            throw new ArgumentException(
                "Payment amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(provider))
            throw new ArgumentException(
                "Payment provider is required.");

        var order = await _orderRepository.GetByIdAsync(
            orderId,
            cancellationToken);

        if (order is null)
            throw new KeyNotFoundException(
                "Order was not found.");

        var existingPayment =
            await _paymentRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        if (existingPayment is not null)
            throw new InvalidOperationException(
                "A payment already exists for this order.");

        if (amount != order.Total)
            throw new InvalidOperationException(
                "Payment amount must match the order total.");

        var payment = new Payment(
            orderId,
            amount,
            provider);

        order.MarkPaymentPending();

        await _paymentRepository.AddAsync(
            payment,
            cancellationToken);

        await _paymentRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(payment);
    }

    public async Task<PaymentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            id,
            cancellationToken);

        return payment is null
            ? null
            : MapToDto(payment);
    }

    public async Task<bool> ProcessAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetPaymentAsync(
            id,
            cancellationToken);

        if (payment is null)
            return false;

        payment.StartProcessing();

        await _paymentRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    public async Task<bool> SucceedAsync(
        Guid id,
        string transactionId,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetPaymentAsync(
            id,
            cancellationToken);

        if (payment is null)
            return false;

        await _commerceTransaction.ExecuteAsync(
            async transactionCancellationToken =>
            {
                payment.MarkSucceeded(transactionId);

                var order =
                    await _orderRepository.GetByIdAsync(
                        payment.OrderId,
                        transactionCancellationToken);

                if (order is null)
                    throw new KeyNotFoundException(
                        "Order was not found.");

                var reservations =
                    await _inventoryReservationRepository
                        .GetByOrderIdAsync(
                            payment.OrderId,
                            transactionCancellationToken);

                if (reservations.Count == 0)
                {
                    throw new KeyNotFoundException(
                        "Inventory reservations were not found.");
                }

                foreach (var reservation in reservations)
                {
                    if (reservation.IsConfirmed ||
                        reservation.IsReleased)
                    {
                        continue;
                    }

                    if (reservation.IsExpired)
                    {
                        throw new InvalidOperationException(
                            "One or more inventory reservations have expired.");
                    }

                    var inventory = reservation.Inventory;

                    if (inventory is null)
                    {
                        throw new KeyNotFoundException(
                            "Inventory was not found.");
                    }

                    inventory.ConfirmReservation(
                        reservation.Quantity);

                    reservation.Confirm();
                }

                order.MarkPaid();
            },
            cancellationToken);

        return true;
    }

    public async Task<bool> FailAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetPaymentAsync(
            id,
            cancellationToken);

        if (payment is null)
            return false;

        await _commerceTransaction.ExecuteAsync(
            async transactionCancellationToken =>
            {
                payment.MarkFailed();

                var order =
                    await _orderRepository.GetByIdAsync(
                        payment.OrderId,
                        transactionCancellationToken);

                if (order is null)
                    throw new KeyNotFoundException(
                        "Order was not found.");

                var reservations =
                    await _inventoryReservationRepository
                        .GetByOrderIdAsync(
                            payment.OrderId,
                            transactionCancellationToken);

                foreach (var reservation in reservations)
                {
                    if (reservation.IsConfirmed ||
                        reservation.IsReleased)
                    {
                        continue;
                    }

                    var inventory = reservation.Inventory;

                    if (inventory is null)
                    {
                        throw new KeyNotFoundException(
                            "Inventory was not found.");
                    }

                    inventory.ReleaseReservation(
                        reservation.Quantity);

                    reservation.Release();
                }

                order.Cancel();
            },
            cancellationToken);

        return true;
    }

    public async Task<bool> RefundAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var payment = await GetPaymentAsync(
            id,
            cancellationToken);

        if (payment is null)
            return false;

        payment.MarkRefunded();

        var order = await _orderRepository.GetByIdAsync(
            payment.OrderId,
            cancellationToken);

        if (order is null)
            throw new KeyNotFoundException(
                "Order was not found.");

        await _paymentRepository.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    private async Task<Payment?> GetPaymentAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _paymentRepository.GetByIdAsync(
            id,
            cancellationToken);
    }

    private static PaymentDto MapToDto(
        Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Provider = payment.Provider,
            ProviderTransactionId =
                payment.ProviderTransactionId,
            Status = payment.Status,
            ProcessedAtUtc =
                payment.ProcessedAtUtc,
            CreatedAtUtc =
                payment.CreatedAtUtc,
            UpdatedAtUtc =
                payment.UpdatedAtUtc
        };
    }
}