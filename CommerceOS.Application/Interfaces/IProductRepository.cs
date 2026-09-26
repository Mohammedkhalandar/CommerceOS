using CommerceOS.Domain.Entities;

namespace CommerceOS.Application.Interfaces;

public interface IProductRepository
{
    Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default);

    void Remove(Product product);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}