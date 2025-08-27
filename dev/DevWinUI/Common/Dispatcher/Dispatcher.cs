using Microsoft.UI.Dispatching;

namespace DevWinUI;
public partial class Dispatcher : IDispatcher
{
    private readonly DispatcherQueue _dispatcher;

    public Dispatcher(Window window)
    {
        _dispatcher = DispatcherQueue.GetForCurrentThread();
    }

    public Dispatcher(UIElement element)
    {
        _dispatcher = DispatcherQueue.GetForCurrentThread();

    }

    /// <inheritdoc />
    public bool TryEnqueue(Action action) => _dispatcher.TryEnqueue(() => action());

    /// <inheritdoc />
    public async ValueTask<TResult> ExecuteAsync<TResult>(AsyncFunc<TResult> func, CancellationToken cancellation)
    {
        if (HasThreadAccess)
        {
            return await func(cancellation);
        }
        return await _dispatcher.ExecuteAsync(func, cancellation);
    }

    /// <inheritdoc />
    public bool HasThreadAccess => _dispatcher.HasThreadAccess;
}
