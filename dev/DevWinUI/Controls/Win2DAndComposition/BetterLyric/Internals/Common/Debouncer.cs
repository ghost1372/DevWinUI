namespace DevWinUI;

internal partial class Debouncer : IDisposable
{
    private long _currentVersion = 0;
    private CancellationTokenSource? _delayCts;
    private CancellationTokenSource? _executeCts;
    private static readonly TimeSpan DebounceTimeout = TimeSpan.FromMilliseconds(250);

    public void Cancel()
    {
        Interlocked.Increment(ref _currentVersion);

        _delayCts?.Cancel();
        _executeCts?.Cancel();
    }

    private Task SafeDelayAsync(int milliseconds, CancellationToken token)
    {
        if (token.IsCancellationRequested) return Task.CompletedTask;

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var timer = new Timer(_ => tcs.TrySetResult(), null, milliseconds, Timeout.Infinite);
        var ctr = token.Register(() => tcs.TrySetResult());

        tcs.Task.ContinueWith(_ =>
        {
            timer.Dispose();
            ctr.Dispose();
        }, TaskContinuationOptions.ExecuteSynchronously);

        return tcs.Task;
    }

    public async Task RunAsync(Action action, int? delayMilliseconds = null)
    {
        long expectedVersion = Interlocked.Increment(ref _currentVersion);

        _delayCts?.Cancel();
        _delayCts?.Dispose();
        _delayCts = new CancellationTokenSource();

        await SafeDelayAsync(delayMilliseconds ?? (int)DebounceTimeout.TotalMilliseconds, _delayCts.Token);

        if (Interlocked.Read(ref _currentVersion) == expectedVersion && !_delayCts.IsCancellationRequested)
        {
            action();
        }
    }
    public void Dispose()
    {
        Cancel();

        _delayCts?.Dispose();
        _executeCts?.Dispose();

        _delayCts = null;
        _executeCts = null;

        GC.SuppressFinalize(this);
    }
}
