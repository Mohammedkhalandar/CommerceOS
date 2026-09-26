using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfCustomerRepository : ICustomerRepository
{
    private readonly CommerceDbContext _dbContext;

    public EfCustomerRepository(CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        await _dbContext.Customers.AddAsync(
            customer,
            cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .Include(x => x.Addresses)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}