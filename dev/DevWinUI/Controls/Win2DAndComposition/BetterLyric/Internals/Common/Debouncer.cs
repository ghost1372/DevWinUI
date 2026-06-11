namespace DevWinUI;

internal partial class Debouncer
{
    private long _currentVersion = 0;
    public static readonly TimeSpan DebounceTimeout = TimeSpan.FromMilliseconds(250);

    public async Task RunAsync(Action action, int? delayMilliseconds = null)
    {
        long expectedVersion = Interlocked.Increment(ref _currentVersion);
        await Task.Delay(delayMilliseconds ?? (int)DebounceTimeout.TotalMilliseconds);

        if (Interlocked.Read(ref _currentVersion) == expectedVersion)
        {
            action();
        }
    }
}
