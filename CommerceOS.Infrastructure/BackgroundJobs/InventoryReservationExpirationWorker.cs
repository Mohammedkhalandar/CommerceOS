using CommerceOS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommerceOS.Infrastructure.BackgroundJobs;

public class InventoryReservationExpirationWorker
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<InventoryReservationExpirationWorker> _logger;

    public InventoryReservationExpirationWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<InventoryReservationExpirationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Inventory reservation expiration worker started.");

        using var timer = new PeriodicTimer(
            TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var reservationService =
                    scope.ServiceProvider
                        .GetRequiredService<IInventoryReservationService>();

                var released =
                    await reservationService.ReleaseExpiredAsync(
                        stoppingToken);

                if (released)
                {
                    _logger.LogInformation(
                        "Expired inventory reservations were released.");
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while releasing expired inventory reservations.");
            }
        }

        _logger.LogInformation(
            "Inventory reservation expiration worker stopped.");
    }
}