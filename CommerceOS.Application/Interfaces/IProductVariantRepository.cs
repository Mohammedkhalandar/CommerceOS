using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Interfaces;

public interface IProductVariantRepository
{
    Task AddAsync(
        ProductVariant variant,
        CancellationToken cancellationToken = default);

    Task<ProductVariant?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProductVariant>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    Task<ProductVariant?> GetBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default);

    void Remove(ProductVariant variant);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}