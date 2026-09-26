using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}