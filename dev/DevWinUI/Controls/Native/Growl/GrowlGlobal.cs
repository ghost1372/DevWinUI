using Microsoft.UI.Windowing;

namespace DevWinUI;

public static partial class GrowlGlobal
{
    private static readonly Dictionary<(ulong Display, GrowlPosition Position), GrowlWindow> Windows = new();

    private static double _spacing = 8;
    private static int _maxVisiblePerPosition;
    private static GrowlHostLayoutMode _layoutMode = GrowlHostLayoutMode.List;
    private static bool _newestOnTop;
    private static double _stackPeekHeight = 14;
    private static int _stackVisibleDepth = 3;
    private static bool _stackExpandOnHover = true;
    private static GrowlShowAnimation _defaultShowAnimation = GrowlShowAnimation.Slide;
    private static GrowlCloseAnimation _defaultCloseAnimation = GrowlCloseAnimation.Fade;
    private static TimeSpan _animationDuration = TimeSpan.FromMilliseconds(300);
    private static GrowlCardColorStyle _defaultColorStyle = GrowlCardColorStyle.Solid;
    private static ScrollBarVisibility _scrollBarVisibility = ScrollBarVisibility.Hidden;
    private static GrowlSound _defaultSound = GrowlSound.None;

    public static GrowlPosition DefaultPosition { get; set; } = GrowlPosition.TopRight;

    public static GrowlMonitor DefaultMonitor { get; set; } = GrowlMonitor.Primary;

    public static Thickness ContentPadding { get; set; } = new(24);

    public static Action<GrowlHost>? ConfigureHost { get; set; }

    public static double Spacing
    {
        get => _spacing;
        set
        {
            _spacing = value;
            Apply(h => h.Spacing = value);
        }
    }

    public static int MaxVisiblePerPosition
    {
        get => _maxVisiblePerPosition;
        set
        {
            _maxVisiblePerPosition = value;
            Apply(h => h.MaxVisiblePerPosition = value);
        }
    }

    public static GrowlHostLayoutMode LayoutMode
    {
        get => _layoutMode;
        set
        {
            _layoutMode = value;
            Apply(h => h.LayoutMode = value);
        }
    }

    public static bool NewestOnTop
    {
        get => _newestOnTop;
        set
        {
            _newestOnTop = value;
            Apply(h => h.NewestOnTop = value);
        }
    }

    public static double StackPeekHeight
    {
        get => _stackPeekHeight;
        set
        {
            _stackPeekHeight = value;
            Apply(h => h.StackPeekHeight = value);
        }
    }

    public static int StackVisibleDepth
    {
        get => _stackVisibleDepth;
        set
        {
            _stackVisibleDepth = value;
            Apply(h => h.StackVisibleDepth = value);
        }
    }

    public static bool StackExpandOnHover
    {
        get => _stackExpandOnHover;
        set
        {
            _stackExpandOnHover = value;
            Apply(h => h.StackExpandOnHover = value);
        }
    }

    public static GrowlShowAnimation DefaultShowAnimation
    {
        get => _defaultShowAnimation;
        set
        {
            _defaultShowAnimation = value;
            Apply(h => h.DefaultShowAnimation = value);
        }
    }

    public static GrowlCloseAnimation DefaultCloseAnimation
    {
        get => _defaultCloseAnimation;
        set
        {
            _defaultCloseAnimation = value;
            Apply(h => h.DefaultCloseAnimation = value);
        }
    }

    public static TimeSpan AnimationDuration
    {
        get => _animationDuration;
        set
        {
            _animationDuration = value;
            Apply(h => h.AnimationDuration = value);
        }
    }

    public static GrowlCardColorStyle DefaultColorStyle
    {
        get => _defaultColorStyle;
        set
        {
            _defaultColorStyle = value;
            Apply(h => h.DefaultColorStyle = value);
        }
    }

    public static ScrollBarVisibility ScrollBarVisibility
    {
        get => _scrollBarVisibility;
        set
        {
            _scrollBarVisibility = value;
            Apply(h => h.ScrollBarVisibility = value);
        }
    }

    public static Microsoft.UI.Dispatching.DispatcherQueue? DispatcherQueue { get; set; }

    public static GrowlSound DefaultSound
    {
        get => _defaultSound;
        set
        {
            _defaultSound = value;
            Apply(h => h.DefaultSound = value);
        }
    }

    public static GrowlHandle Show(GrowlOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var queue = DispatcherQueue ??= Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        if (queue is null || !queue.HasThreadAccess)
        {
            throw new InvalidOperationException("GrowlGlobal.Show must be called on the UI thread. Use Growl.ShowAsync from other threads.");
        }

        var position = options.Position ?? DefaultPosition;
        var area = ResolveArea(options.Monitor ?? DefaultMonitor);
        var key = (area.DisplayId.Value, position);

        if (!Windows.TryGetValue(key, out var window))
        {
            window = new GrowlWindow(position, area.DisplayId);
            window.Closed += (_, _) => Windows.Remove(key);

            ApplySettings(window.Host);
            ConfigureHost?.Invoke(window.Host);

            Windows[key] = window;
        }

        return window.Show(options);
    }

    public static void ClearAll()
    {
        if (!CheckUiThread(nameof(ClearAll)))
        {
            return;
        }

        foreach (var window in Windows.Values.ToList())
        {
            window.ClearAll();
        }
    }

    public static void Shutdown()
    {
        if (!CheckUiThread(nameof(Shutdown)))
        {
            return;
        }

        foreach (var window in Windows.Values.ToList())
        {
            window.Close();
        }

        Windows.Clear();
    }

    private static bool CheckUiThread(string member)
    {
        var queue = DispatcherQueue;

        if (queue is null)
        {
            return false;
        }

        if (!queue.HasThreadAccess)
        {
            throw new InvalidOperationException($"GrowlGlobal.{member} must be called on the UI thread.");
        }

        return true;
    }

    private static void Apply(Action<GrowlHost> action)
    {
        foreach (var window in Windows.Values)
        {
            action(window.Host);
        }
    }

    private static void ApplySettings(GrowlHost host)
    {
        host.Spacing = _spacing;
        host.MaxVisiblePerPosition = _maxVisiblePerPosition;
        host.LayoutMode = _layoutMode;
        host.NewestOnTop = _newestOnTop;
        host.StackPeekHeight = _stackPeekHeight;
        host.StackVisibleDepth = _stackVisibleDepth;
        host.StackExpandOnHover = _stackExpandOnHover;
        host.DefaultShowAnimation = _defaultShowAnimation;
        host.DefaultCloseAnimation = _defaultCloseAnimation;
        host.AnimationDuration = _animationDuration;
        host.DefaultColorStyle = _defaultColorStyle;
        host.ScrollBarVisibility = _scrollBarVisibility;
        host.DefaultSound = _defaultSound;
    }

    private static DisplayArea ResolveArea(GrowlMonitor monitor)
    {
        if (monitor == GrowlMonitor.Cursor && DisplayMonitorHelper.TryGetDisplayAreaAtCursor(out var area) && area is not null)
        {
            return area;
        }

        return DisplayArea.Primary;
    }
}
