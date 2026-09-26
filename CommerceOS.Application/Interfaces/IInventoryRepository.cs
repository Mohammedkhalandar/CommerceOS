using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Inventory?> GetByVariantIdAsync(
        Guid productVariantId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Inventory inventory,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}