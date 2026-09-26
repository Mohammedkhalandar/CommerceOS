using System;
using System.Threading;
using System.Threading.Tasks;

namespace CommerceOS.Application.BackgroundJobs;

public interface IBackgroundJobQueue
{
    ValueTask QueueAsync(
        Func<CancellationToken, ValueTask> workItem);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken);
}