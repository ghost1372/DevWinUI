namespace DevWinUI;
internal static partial class Disposable
{
    public static readonly IDisposable Empty = new EmptyDisposable();

    public static IDisposable Create(Action disposeAction)
        => new AnonymousDisposable(disposeAction);

    private sealed partial class EmptyDisposable : IDisposable
    {
        public void Dispose() { }
    }

    private sealed partial class AnonymousDisposable : IDisposable
    {
        private readonly Action _dispose;
        private bool _isDisposed;

        public AnonymousDisposable(Action dispose)
        {
            _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _dispose();
            }
        }
    }
}
