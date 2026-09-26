using CommerceOS.Application.DTOs;
using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Services;

public class InventoryReservationService
    : IInventoryReservationService
{
    private readonly IInventoryReservationRepository
        _reservationRepository;

    public InventoryReservationService(
        IInventoryReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<IReadOnlyList<InventoryReservationDto>>
        GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
    {
        var reservations =
            await _reservationRepository.GetByOrderIdAsync(
                orderId,
                cancellationToken);

        return reservations
            .Select(MapToDto)
            .ToList();
    }

    public async Task<IReadOnlyList<InventoryReservationDto>>
        GetByInventoryIdAsync(
            Guid inventoryId,
            CancellationToken cancellationToken = default)
    {
        var reservations =
            await _reservationRepository.GetByInventoryIdAsync(
                inventoryId,
                cancellationToken);

        return reservations
            .Select(MapToDto)
            .ToList();
    }

    public async Task<bool> ReleaseExpiredAsync(
        CancellationToken cancellationToken = default)
    {
        var releasedAny = false;

        var reservations =
            await _reservationRepository.GetExpiredAsync(
                cancellationToken);

        foreach (var reservation in reservations)
        {
            if (reservation.IsConfirmed ||
                reservation.IsReleased)
            {
                continue;
            }

            var inventory = reservation.Inventory;

            if (inventory is null)
                continue;

            inventory.ReleaseReservation(
                reservation.Quantity);

            reservation.Release();

            releasedAny = true;
        }

        if (releasedAny)
        {
            await _reservationRepository.SaveChangesAsync(
                cancellationToken);
        }

        return releasedAny;
    }

    private static InventoryReservationDto MapToDto(
        InventoryReservation reservation)
    {
        return new InventoryReservationDto
        {
            Id = reservation.Id,
            InventoryId = reservation.InventoryId,
            OrderId = reservation.OrderId,
            Quantity = reservation.Quantity,
            ExpiresAtUtc = reservation.ExpiresAtUtc,
            IsReleased = reservation.IsReleased,
            IsConfirmed = reservation.IsConfirmed,
            IsExpired = reservation.IsExpired,
            CreatedAtUtc = reservation.CreatedAtUtc,
            UpdatedAtUtc = reservation.UpdatedAtUtc
        };
    }
}