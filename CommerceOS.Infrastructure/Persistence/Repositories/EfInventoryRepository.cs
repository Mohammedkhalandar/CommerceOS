using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using CommerceOS.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfInventoryRepository : IInventoryRepository
{
    private readonly CommerceDbContext _dbContext;

    public EfInventoryRepository(
        CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Inventory?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Inventories
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Inventory?> GetByVariantIdAsync(
        Guid productVariantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Inventories
            .FirstOrDefaultAsync(
                x => x.ProductVariantId == productVariantId,
                cancellationToken);
    }

    public async Task AddAsync(
        Inventory inventory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inventory);

        await _dbContext.Inventories.AddAsync(
            inventory,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InventoryConcurrencyException(
                "Inventory was changed by another request. Please refresh the inventory and try again.",
                ex);
        }
    }
}