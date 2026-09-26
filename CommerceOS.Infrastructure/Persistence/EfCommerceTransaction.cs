using CommerceOS.Application.Interfaces;
using CommerceOS.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CommerceOS.Infrastructure.Persistence;

public sealed class EfCommerceTransaction
    : ICommerceTransaction
{
    private readonly CommerceDbContext _dbContext;

    public EfCommerceTransaction(
        CommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            await operation(cancellationToken);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw new InventoryConcurrencyException(
                "Inventory or another resource was changed by another request. Please refresh and try again.",
                ex);
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }
}