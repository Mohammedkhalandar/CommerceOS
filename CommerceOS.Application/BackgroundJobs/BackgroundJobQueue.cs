using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CommerceOS.Application.BackgroundJobs;

public class BackgroundJobQueue : IBackgroundJobQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public BackgroundJobQueue()
    {
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        };

        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueAsync(
        Func<CancellationToken, ValueTask> workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}