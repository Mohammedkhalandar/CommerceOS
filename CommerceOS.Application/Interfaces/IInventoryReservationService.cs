using CommerceOS.Application.DTOs;

namespace CommerceOS.Application.Interfaces;

public interface IInventoryReservationService
{
    Task<IReadOnlyList<InventoryReservationDto>>
        GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryReservationDto>>
        GetByInventoryIdAsync(
            Guid inventoryId,
            CancellationToken cancellationToken = default);

    Task<bool> ReleaseExpiredAsync(
        CancellationToken cancellationToken = default);
}