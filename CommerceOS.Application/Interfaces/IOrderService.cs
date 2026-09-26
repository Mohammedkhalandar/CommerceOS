using CommerceOS.Application.DTOs.Orders;

namespace CommerceOS.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(
        CreateOrderDto request,
        CancellationToken cancellationToken = default);

    Task<OrderDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrderDto>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<bool> MarkPaymentPendingAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> MarkPaidAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> StartProcessingAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ShipAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> DeliverAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}