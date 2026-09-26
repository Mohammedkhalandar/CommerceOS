using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfProductVariantRepository : IProductVariantRepository
{
    private readonly CommerceDbContext _dbContext;

    public EfProductVariantRepository(CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        ProductVariant variant,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(variant);

        await _dbContext.ProductVariants.AddAsync(
            variant,
            cancellationToken);
    }

    public async Task<ProductVariant?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductVariants
            .Include(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductVariants
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.Sku)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductVariant?> GetBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductVariants
            .FirstOrDefaultAsync(
                x => x.Sku == sku,
                cancellationToken);
    }

    public void Remove(ProductVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        _dbContext.ProductVariants.Remove(variant);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}