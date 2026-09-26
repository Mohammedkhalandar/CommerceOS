using CommerceOS.Domain.Enums;

namespace CommerceOS.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Provider { get; set; } = null!;

    public string? ProviderTransactionId { get; set; }

    public PaymentStatus Status { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}