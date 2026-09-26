using CommerceOS.Domain.Common;
using CommerceOS.Domain.Enums;

namespace CommerceOS.Domain.Entities;

public class Payment : BaseEntity
{
    private Payment()
    {
    }

    public Payment(
        Guid orderId,
        decimal amount,
        string provider)
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

        OrderId = orderId;
        Amount = amount;
        Provider = provider;
        Status = PaymentStatus.Pending;
    }

    public Guid OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public string Provider { get; private set; } = null!;

    public string? ProviderTransactionId { get; private set; }

    public PaymentStatus Status { get; private set; }

    public DateTime? ProcessedAtUtc { get; private set; }

    public Order Order { get; private set; } = null!;

    public void StartProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException(
                "Payment is not pending.");

        Status = PaymentStatus.Processing;

        MarkUpdated();
    }

    public void MarkSucceeded(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException(
                "Transaction ID is required.");

        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException(
                "Payment is not being processed.");

        ProviderTransactionId = transactionId;
        Status = PaymentStatus.Succeeded;
        ProcessedAtUtc = DateTime.UtcNow;

        MarkUpdated();
    }

    public void MarkFailed()
    {
        if (Status == PaymentStatus.Succeeded)
            throw new InvalidOperationException(
                "Successful payment cannot be marked as failed.");

        Status = PaymentStatus.Failed;
        ProcessedAtUtc = DateTime.UtcNow;

        MarkUpdated();
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Succeeded)
            throw new InvalidOperationException(
                "Only successful payments can be refunded.");

        Status = PaymentStatus.Refunded;
        ProcessedAtUtc = DateTime.UtcNow;

        MarkUpdated();
    }
}