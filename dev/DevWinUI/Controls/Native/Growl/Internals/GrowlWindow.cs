using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using DisplayId = Microsoft.UI.DisplayId;

namespace DevWinUI;

internal sealed partial class GrowlWindow : Window
{
    private readonly GrowlPosition _position;
    private readonly DisplayId _displayId;
    private readonly DispatcherQueueTimer _shrinkTimer;

    private bool _activated;
    private bool _closed;
    private bool _repositionQueued;
    private DateTime _lingerUntil;
    private int _x;
    private int _y;
    private int _width;
    private int _height;

    public GrowlWindow(GrowlPosition position, DisplayId displayId)
    {
        _position = position;
        _displayId = displayId;

        SystemBackdrop = new TransparentBackdrop();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;
        AppWindow.IsShownInSwitchers = false;

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsAlwaysOnTop = true;
        }

        WindowHelper.RemoveWindowBorderAndTitleBar(this);

        Host = new GrowlHost
        {
            DefaultPosition = position,
            Padding = GrowlGlobal.ContentPadding,
            SuppressRegistration = true
        };

        Content = Host;

        _shrinkTimer = DispatcherQueue.CreateTimer();
        _shrinkTimer.IsRepeating = false;
        _shrinkTimer.Tick += OnShrinkTick;

        Host.LayoutChanged += _ => QueueReposition();
        Host.Loaded += (_, _) =>
        {
            if (Host.XamlRoot is { } root)
            {
                root.Changed += (_, _) => QueueReposition();
            }

            QueueReposition();
        };

        Closed += (_, _) =>
        {
            _closed = true;
            _shrinkTimer.Stop();
            Host.Abandon();
        };

        Host.ApplyTemplate();
    }

    public GrowlHost Host { get; }

    public GrowlHandle Show(GrowlOptions options)
    {
        _shrinkTimer.Stop();

        var handle = Host.Show(options);
        handle.Closed -= OnHandleClosed;
        handle.Closed += OnHandleClosed;

        Reposition(false);

        if (!_activated)
        {
            _activated = true;
            AppWindow.Show(false);
        }

        return handle;
    }

    public void ClearAll()
    {
        Host.ClearAll();
    }

    private void QueueReposition()
    {
        if (_repositionQueued || _closed)
        {
            return;
        }

        _repositionQueued = true;

        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, () =>
        {
            _repositionQueued = false;
            Reposition(false);
        });
    }

    private void Reposition(bool allowShrink)
    {
        if (_closed)
        {
            return;
        }

        var area = DisplayArea.GetFromDisplayId(_displayId) ?? DisplayArea.Primary;
        var work = area.WorkArea;

        double scale = Host.XamlRoot?.RasterizationScale ?? 1.0;

        if (scale <= 0)
        {
            scale = 1.0;
        }

        Host.Measure(new Size(work.Width / scale, work.Height / scale));

        var desired = Host.DesiredSize;
        int width = (int)Math.Ceiling(desired.Width * scale);
        int height = (int)Math.Ceiling(desired.Height * scale);

        if (width <= 0 || height <= 0)
        {
            return;
        }

        width = Math.Min(width, work.Width);
        height = Math.Min(height, work.Height);

        if (!allowShrink)
        {
            if ((width < _width || height < _height) && !_shrinkTimer.IsRunning)
            {
                _shrinkTimer.Interval = Host.AnimationDuration + TimeSpan.FromMilliseconds(100);
                _shrinkTimer.Start();
            }

            width = Math.Max(width, _width);
            height = Math.Max(height, _height);
        }

        bool top = _position is GrowlPosition.TopLeft or GrowlPosition.TopCenter or GrowlPosition.TopRight;

        int x = _position switch
        {
            GrowlPosition.TopLeft or GrowlPosition.BottomLeft => work.X,
            GrowlPosition.TopCenter or GrowlPosition.BottomCenter => work.X + (work.Width - width) / 2,
            _ => work.X + work.Width - width
        };

        int y = top ? work.Y : work.Y + work.Height - height;

        if (width == _width && height == _height && x == _x && y == _y)
        {
            return;
        }

        _x = x;
        _y = y;
        _width = width;
        _height = height;

        AppWindow.MoveAndResize(new RectInt32(x, y, width, height));
    }

    private void OnHandleClosed(GrowlHandle handle, object? args)
    {
        if (_closed)
        {
            return;
        }

        var duration = Host.AnimationDuration;
        var linger = handle.CloseAnimation == GrowlCloseAnimation.Snap
            ? TimeSpan.FromTicks(duration.Ticks * 5) + TimeSpan.FromMilliseconds(150)
            : duration + TimeSpan.FromMilliseconds(100);

        var until = DateTime.UtcNow + linger;

        if (until > _lingerUntil)
        {
            _lingerUntil = until;
        }

        _shrinkTimer.Stop();
        _shrinkTimer.Interval = _lingerUntil - DateTime.UtcNow;
        _shrinkTimer.Start();
    }

    private void OnShrinkTick(DispatcherQueueTimer sender, object args)
    {
        sender.Stop();

        if (Host.HandleCount == 0)
        {
            Close();
            return;
        }

        Reposition(true);
    }
}
