using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Interfaces;

public interface IInventoryReservationRepository
{
    Task AddAsync(
        InventoryReservation reservation,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryReservation>>
        GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryReservation>>
        GetByInventoryIdAsync(
            Guid inventoryId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryReservation>>
        GetExpiredAsync(
            CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}