namespace DevWinUI;

[TemplatePart(Name = "PART_TopLeft", Type = typeof(Panel))]
[TemplatePart(Name = "PART_TopCenter", Type = typeof(Panel))]
[TemplatePart(Name = "PART_TopRight", Type = typeof(Panel))]
[TemplatePart(Name = "PART_BottomLeft", Type = typeof(Panel))]
[TemplatePart(Name = "PART_BottomCenter", Type = typeof(Panel))]
[TemplatePart(Name = "PART_BottomRight", Type = typeof(Panel))]
public sealed partial class GrowlHost : Control
{
    private readonly List<GrowlHandle> _handles = new();
    private readonly Dictionary<GrowlPosition, Panel> _panels = new();
    private readonly HashSet<GrowlPosition> _expanded = new();
    private bool _abandoned;

    public GrowlHost()
    {
        DefaultStyleKey = typeof(GrowlHost);

        Loaded += (_, _) =>
        {
            if (!SuppressRegistration)
            {
                Growl.Register(this);
            }
        };
        Unloaded += (_, _) =>
        {
            if (!SuppressRegistration)
            {
                Growl.Unregister(this);
            }
        };
    }

    internal bool SuppressRegistration { get; init; }

    internal int HandleCount => _handles.Count;

    internal event Action<GrowlHost>? LayoutChanged;

    public GrowlHandle Show(GrowlOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!DispatcherQueue.HasThreadAccess)
        {
            throw new InvalidOperationException("GrowlHost.Show must be called on the UI thread. Use Growl.ShowAsync from other threads.");
        }

        if (!string.IsNullOrEmpty(options.Key) && _handles.Find(h => !h.IsClosed && !h.Card.IsClosing && h.Key == options.Key) is { } existing)
        {
            existing.ApplyOptions(options);
            return existing;
        }

        var position = options.Position ?? DefaultPosition;
        var card = CreateCard(options, position, DefaultColorStyle);
        var handle = new GrowlHandle(this, card, position)
        {
            Key = options.Key ?? string.Empty,
            ShowAnimation = options.ShowAnimation ?? DefaultShowAnimation,
            CloseAnimation = options.CloseAnimation ?? DefaultCloseAnimation,
            Sound = options.Sound ?? (options.SoundUri is not null ? GrowlSound.Auto : DefaultSound),
            SoundUri = options.SoundUri
        };

        card.CloseAnimator = done => GrowlAnimator.Close(card, handle.CloseAnimation, position, AnimationDuration, done);

        card.Invoked += (_, _) =>
        {
            handle.RaiseInvoked();
            options.Invoked?.Invoke(handle);
        };

        card.Closed += (_, _) => OnCardClosed(handle);
        card.SizeChanged += (_, _) => ApplyLayout(position);

        _handles.Add(handle);
        Pump(position);

        return handle;
    }

    public GrowlHandle Show(GrowlCardSeverity severity, string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose)
    {
        return Show(new GrowlOptions
        {
            Title = title,
            SubTitle = subTitle,
            Message = message,
            Severity = severity,
            ShowCloseButton = canClose,
            AutoClose = autoClose
        });
    }

    public GrowlHandle ShowSuccess(string title) => Show(GrowlCardSeverity.Success, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowSuccess(string title, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Success, title, string.Empty, null, canClose, autoClose);
    public GrowlHandle ShowSuccess(string title, string subTitle) => Show(GrowlCardSeverity.Success, title, subTitle, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowSuccess(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Success, title, subTitle, null, canClose, autoClose);
    public GrowlHandle ShowSuccess(string title, string subTitle, object? message) => Show(GrowlCardSeverity.Success, title, subTitle, message, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowSuccess(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Success, title, subTitle, message, canClose, autoClose);

    public GrowlHandle ShowWarning(string title) => Show(GrowlCardSeverity.Warning, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowWarning(string title, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Warning, title, string.Empty, null, canClose, autoClose);
    public GrowlHandle ShowWarning(string title, string subTitle) => Show(GrowlCardSeverity.Warning, title, subTitle, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowWarning(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Warning, title, subTitle, null, canClose, autoClose);
    public GrowlHandle ShowWarning(string title, string subTitle, object? message) => Show(GrowlCardSeverity.Warning, title, subTitle, message, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowWarning(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Warning, title, subTitle, message, canClose, autoClose);

    public GrowlHandle ShowError(string title) => Show(GrowlCardSeverity.Error, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowError(string title, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Error, title, string.Empty, null, canClose, autoClose);
    public GrowlHandle ShowError(string title, string subTitle) => Show(GrowlCardSeverity.Error, title, subTitle, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowError(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Error, title, subTitle, null, canClose, autoClose);
    public GrowlHandle ShowError(string title, string subTitle, object? message) => Show(GrowlCardSeverity.Error, title, subTitle, message, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowError(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Error, title, subTitle, message, canClose, autoClose);

    public GrowlHandle ShowInfo(string title) => Show(GrowlCardSeverity.Info, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowInfo(string title, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Info, title, string.Empty, null, canClose, autoClose);
    public GrowlHandle ShowInfo(string title, string subTitle) => Show(GrowlCardSeverity.Info, title, subTitle, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowInfo(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Info, title, subTitle, null, canClose, autoClose);
    public GrowlHandle ShowInfo(string title, string subTitle, object? message) => Show(GrowlCardSeverity.Info, title, subTitle, message, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowInfo(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Info, title, subTitle, message, canClose, autoClose);

    public GrowlHandle ShowAttention(string title) => Show(GrowlCardSeverity.Attention, title, string.Empty, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowAttention(string title, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Attention, title, string.Empty, null, canClose, autoClose);
    public GrowlHandle ShowAttention(string title, string subTitle) => Show(GrowlCardSeverity.Attention, title, subTitle, null, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowAttention(string title, string subTitle, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Attention, title, subTitle, null, canClose, autoClose);
    public GrowlHandle ShowAttention(string title, string subTitle, object? message) => Show(GrowlCardSeverity.Attention, title, subTitle, message, true, GrowlCardAutoClose.None);
    public GrowlHandle ShowAttention(string title, string subTitle, object? message, bool canClose, GrowlCardAutoClose autoClose) => Show(GrowlCardSeverity.Attention, title, subTitle, message, canClose, autoClose);

    public void Clear(GrowlPosition position)
    {
        EnsureUiThread();
        CloseHandles(_handles.Where(h => h.Position == position));
    }

    public void ClearAll()
    {
        EnsureUiThread();
        CloseHandles(_handles);
    }

    internal void Abandon()
    {
        _abandoned = true;

        foreach (var handle in _handles.ToList())
        {
            handle.Card.CloseAnimator = null;
            handle.Card.IsOpen = false;
        }
    }

    private void EnsureUiThread()
    {
        if (!DispatcherQueue.HasThreadAccess)
        {
            throw new InvalidOperationException("GrowlHost methods must be called on the UI thread.");
        }
    }

    private static void CloseHandles(IEnumerable<GrowlHandle> handles)
    {
        foreach (var handle in handles.OrderBy(h => h.IsShown).ToList())
        {
            handle.Close();
        }
    }

    internal void Dismiss(GrowlHandle handle)
    {
        if (handle.IsClosed)
        {
            return;
        }

        if (!handle.IsShown)
        {
            handle.Card.CloseAnimator = null;
        }

        handle.Card.Close();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        foreach (var handle in _handles.Where(h => h.IsShown))
        {
            if (handle.Card.Parent is Panel oldPanel)
            {
                oldPanel.Children.Remove(handle.Card);
            }

            handle.IsShown = false;
        }

        _panels.Clear();
        _expanded.Clear();

        foreach (var position in Enum.GetValues<GrowlPosition>())
        {
            if (GetTemplateChild($"PART_{position}") is Panel panel)
            {
                _panels[position] = panel;
                HookPanel(panel, position);
            }
        }

        foreach (var position in Enum.GetValues<GrowlPosition>())
        {
            Pump(position);
        }
    }

    private static GrowlCard CreateCard(GrowlOptions options, GrowlPosition position, GrowlCardColorStyle defaultColorStyle)
    {
        var card = new GrowlCard
        {
            Title = options.Title,
            SubTitle = options.SubTitle,
            Content = options.Message,
            Icon = options.Icon,
            IconBackground = options.IconBackground,
            IconForeground = options.IconForeground,
            IconBorderBrush = options.IconBorderBrush,
            IconStyle = options.IconStyle,
            ShowIconContainer = options.ShowIconContainer,
            ActionContent = options.ActionContent,
            Severity = options.Severity,
            ColorStyle = options.ColorStyle ?? defaultColorStyle,
            IndicatorPlacement = options.IndicatorPlacement,
            ShowIndicator = options.ShowIndicator,
            ShowGradientBackground = options.ShowGradientBackground,
            GradientIntensity = options.GradientIntensity,
            DisplayMode = options.DisplayMode,
            AutoClose = options.AutoClose,
            AutoCloseDuration = options.AutoCloseDuration,
            CountdownSeverity = options.CountdownSeverity,
            CountdownColorStyle = options.CountdownColorStyle,
            PauseAutoCloseOnPointerOver = options.PauseAutoCloseOnPointerOver,
            ShowCloseButton = options.ShowCloseButton,
            CloseOnInvoke = options.CloseOnInvoke,
            SentTimeVisibility = options.SentTimeVisibility,
            SentAt = options.SentAt,
            CountdownPlacement = options.CountdownPlacement,
            ShowCountdown = options.ShowCountdown,
            ShowCountdownText = options.ShowCountdownText,
            IndicatorThickness = options.IndicatorThickness,
            ShowProgress = options.ShowProgress,
            ShowProgressText = options.ShowProgressText,
            ProgressType = options.ProgressType,
            ProgressMaximum = options.ProgressMaximum,
            Progress = options.Progress,
            IsProgressIndeterminate = options.IsProgressIndeterminate,
            IsProgressError = options.IsProgressError,
            IsProgressPaused = options.IsProgressPaused,
            HorizontalAlignment = options.DisplayMode == GrowlCardDisplayMode.Banner
                ? HorizontalAlignment.Stretch
                : GetAlignment(position)
        };

        if (options.Width is { } width)
        {
            card.Width = width;
        }

        options.Configure?.Invoke(card);

        return card;
    }

    private void OnCardClosed(GrowlHandle handle)
    {
        if (handle.IsClosed)
        {
            return;
        }

        if (!_abandoned && handle.IsShown && _panels.TryGetValue(handle.Position, out var panel))
        {
            RemoveWithShift(panel, handle.Card);
        }

        _handles.Remove(handle);
        handle.MarkClosed();

        if (!_abandoned)
        {
            Pump(handle.Position);
        }
    }

}
