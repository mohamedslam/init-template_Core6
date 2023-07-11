using Fab.Utils.Extensions;

namespace Fab.Utils.Threading;

public static class TaskExtensions
{
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        // This disposes the registration as soon as one of the tasks trigger
        await using var registration = cancellationToken.Register(
            state => state!.As<TaskCompletionSource<object?>>()
                           .TrySetResult(null),
            tcs);

        var resultTask = await Task.WhenAny(task, tcs.Task);
        if (resultTask == tcs.Task)
        {
            // Operation cancelled
            throw new OperationCanceledException(cancellationToken);
        }
        
        return await task;
    }

    public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource();
        var delayTask = Task.Delay(timeout, cts.Token);

        var resultTask = await Task.WhenAny(task, delayTask);
        if (resultTask == delayTask)
        {
            // Operation cancelled
            throw new OperationCanceledException();
        }

        // Cancel the timer task so that it does not fire
        cts.Cancel();

        return await task;
    }
}