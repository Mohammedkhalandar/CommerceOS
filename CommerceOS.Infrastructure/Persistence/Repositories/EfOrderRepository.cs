using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfOrderRepository : IOrderRepository
{
    private readonly CommerceDbContext _dbContext;

    public EfOrderRepository(CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        await _dbContext.Orders.AddAsync(
            order,
            cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}