
namespace CommerceOS.Application.Interfaces;

public interface ICommerceTransaction
{
    Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);
}