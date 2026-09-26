using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence.Repositories;

public class EfPaymentRepository : IPaymentRepository
{
    private readonly CommerceDbContext _dbContext;

    public EfPaymentRepository(CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payment);

        await _dbContext.Payments.AddAsync(
            payment,
            cancellationToken);
    }

    public async Task<Payment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Payment?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments
            .FirstOrDefaultAsync(
                x => x.OrderId == orderId,
                cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}