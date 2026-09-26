using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfInventoryReservationRepository
    : IInventoryReservationRepository
{
    private readonly CommerceDbContext _dbContext;

    public EfInventoryReservationRepository(
        CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        InventoryReservation reservation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reservation);

        await _dbContext.InventoryReservations.AddAsync(
            reservation,
            cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryReservation>>
        GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.InventoryReservations
            .Include(x => x.Inventory)
            .Where(x => x.OrderId == orderId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryReservation>>
        GetByInventoryIdAsync(
            Guid inventoryId,
            CancellationToken cancellationToken = default)
    {
        return await _dbContext.InventoryReservations
            .AsNoTracking()
            .Where(x => x.InventoryId == inventoryId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryReservation>>
        GetExpiredAsync(
            CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.InventoryReservations
            .Include(x => x.Inventory)
            .Where(x =>
                x.ExpiresAtUtc <= now &&
                !x.IsReleased &&
                !x.IsConfirmed)
            .OrderBy(x => x.ExpiresAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}