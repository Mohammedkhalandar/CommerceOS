using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfProductRepository : IProductRepository
{
    private readonly CommerceDbContext _dbContext;

    public EfProductRepository(CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(
            product,
            cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public void Remove(Product product)
    {
        _dbContext.Products.Remove(product);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}