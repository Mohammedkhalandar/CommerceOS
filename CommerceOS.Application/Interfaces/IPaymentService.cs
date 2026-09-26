using CommerceOS.Application.DTOs;

namespace CommerceOS.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreateAsync(
        Guid orderId,
        decimal amount,
        string provider,
        CancellationToken cancellationToken = default);

    Task<PaymentDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ProcessAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> SucceedAsync(
        Guid id,
        string transactionId,
        CancellationToken cancellationToken = default);

    Task<bool> FailAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> RefundAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}