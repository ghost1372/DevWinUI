namespace DevWinUI;

public sealed partial class GrowlHandle
{
    private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);

    internal GrowlHandle(GrowlHost host, GrowlCard card, GrowlPosition position)
    {
        Host = host;
        Card = card;
        Position = position;
    }

    public GrowlHost Host { get; }

    public GrowlCard Card { get; }

    public GrowlPosition Position { get; }

    public string Key { get; internal init; } = string.Empty;

    internal GrowlShowAnimation ShowAnimation { get; init; }

    internal GrowlCloseAnimation CloseAnimation { get; init; }

    internal GrowlSound Sound { get; init; }

    internal Uri? SoundUri { get; init; }

    public bool IsShown { get; internal set; }

    public bool IsClosed { get; private set; }

    public Task ClosedTask => _closed.Task;

    public event TypedEventHandler<GrowlHandle, object?>? Closed;

    public event TypedEventHandler<GrowlHandle, object?>? Invoked;

    public Task WaitForCloseAsync(CancellationToken cancellationToken = default)
    {
        return cancellationToken.CanBeCanceled ? _closed.Task.WaitAsync(cancellationToken) : _closed.Task;
    }

    public void Close()
    {
        Run(() => Host.Dismiss(this));
    }

    public void Update(
        string? title = null,
        string? subTitle = null,
        object? message = null,
        GrowlCardSeverity? severity = null,
        double? progress = null,
        bool? isProgressIndeterminate = null,
        bool? showProgress = null,
        bool restartAutoClose = false)
    {
        Run(() =>
        {
            if (title is not null)
            {
                Card.Title = title;
            }

            if (subTitle is not null)
            {
                Card.SubTitle = subTitle;
            }

            if (message is not null)
            {
                Card.Content = message;
            }

            if (severity is { } severityValue)
            {
                Card.Severity = severityValue;
            }

            if (showProgress is { } showBar)
            {
                Card.ShowProgress = showBar;
            }

            if (isProgressIndeterminate is { } indeterminate)
            {
                Card.IsProgressIndeterminate = indeterminate;
            }

            if (progress is { } value)
            {
                Card.Progress = value;
            }

            if (restartAutoClose)
            {
                Card.RestartAutoClose();
            }
        });
    }

    public void Update(Action<GrowlCard> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        Run(() => configure(Card));
    }

    internal void ApplyOptions(GrowlOptions options)
    {
        Update(options.Title, options.SubTitle, options.Message, options.Severity, options.Progress, options.IsProgressIndeterminate, options.ShowProgress, true);
    }

    internal void RaiseInvoked()
    {
        Invoked?.Invoke(this, null);
    }

    internal void MarkClosed()
    {
        IsClosed = true;
        IsShown = false;

        try
        {
            Closed?.Invoke(this, null);
        }
        finally
        {
            _closed.TrySetResult();
        }
    }

    private void Run(Action action)
    {
        if (IsClosed)
        {
            return;
        }

        var queue = Card.DispatcherQueue;

        if (queue.HasThreadAccess)
        {
            action();
        }
        else
        {
            queue.TryEnqueue(() =>
            {
                if (!IsClosed)
                {
                    action();
                }
            });
        }
    }
}
