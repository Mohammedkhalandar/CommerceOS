using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommerceOS.Application.BackgroundJobs;

public class OrderBackgroundWorker : BackgroundService
{
    private readonly IBackgroundJobQueue _queue;
    private readonly ILogger<OrderBackgroundWorker> _logger;

    public OrderBackgroundWorker(
        IBackgroundJobQueue queue,
        ILogger<OrderBackgroundWorker> logger)
    {
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "CommerceOS Order Background Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Application is shutting down.
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while executing background job.");
            }
        }

        _logger.LogInformation(
            "CommerceOS Order Background Worker stopped.");
    }
}