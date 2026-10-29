using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public sealed partial class OrbitCardStack : Control
{
    static readonly TimeSpan TweenDuration = TimeSpan.FromMilliseconds(420);
    static readonly TimeSpan SettleDelay = TweenDuration;

    public event TypedEventHandler<OrbitCardStack, int>? ValueChanged;
    public event TypedEventHandler<OrbitCardStack, bool>? OpenChanged;
    public event TypedEventHandler<OrbitCardStack, OrbitCardInvokedEventArgs>? ItemInvoked;

    readonly List<OrbitCard> _cards = new();
    DispatcherQueueTimer? _settleTimer;
    Grid? _stage;
    double _w, _h;
    int _active;
    bool _init, _open, _hovered, _settling;

    public OrbitCardStack()
    {
        DefaultStyleKey = typeof(OrbitCardStack);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    int ActiveIndex => _cards.Count == 0 ? 0 : Math.Clamp(Value ?? _active, 0, _cards.Count - 1);

    bool IsOpen => Open ?? _open;

    (double cw, double ch, double h, double name, double body) Metrics => Size switch
    {
        OrbitCardSize.Small => (112.0, 148.0, 260.0, 12.0, 10.5),
        OrbitCardSize.Large => (184.0, 244.0, 440.0, 16.0, 12.5),
        _ => (148.0, 196.0, 340.0, 14.0, 11.5),
    };

    protected override void OnApplyTemplate()
    {
        if (_stage is not null)
        {
            _stage.SizeChanged -= OnStageSizeChanged;
            _stage.PointerExited -= OnStagePointerExited;
            _stage.LostFocus -= OnStageLostFocus;
        }

        base.OnApplyTemplate();

        _stage = GetTemplateChild("PART_Stage") as Grid;
        if (_stage is null) return;

        _stage.SizeChanged += OnStageSizeChanged;
        _stage.PointerExited += OnStagePointerExited;
        _stage.LostFocus += OnStageLostFocus;
        Rebuild();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new OrbitCardStackPeer(this);

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_init) return;
        _init = true;

        if (Value is null) _active = DefaultValue;
        if (Open is null) _open = DefaultOpen;
        Relayout(false);
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _settleTimer?.Stop();
        _settling = false;
        _hovered = false;
    }

    void OnStageSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _w = e.NewSize.Width;
        _h = e.NewSize.Height;
        Relayout(false);
    }

    void OnStageLostFocus(object sender, RoutedEventArgs e) => DispatcherQueue.TryEnqueue(() =>
    {
        if (!_hovered && !FocusInsideCards()) Request(ActiveIndex, false);
    });

    void BeginSettle()
    {
        _settling = true;

        if (_settleTimer is null)
        {
            _settleTimer = DispatcherQueue.CreateTimer();
            _settleTimer.Interval = SettleDelay;
            _settleTimer.IsRepeating = false;
            _settleTimer.Tick += (_, _) => _settling = false;
        }

        _settleTimer.Stop();
        _settleTimer.Start();
    }

    void Rebuild()
    {
        if (_stage is null) return;

        var m = Metrics;
        _stage.Height = m.h;
        AutomationProperties.SetName(this, Labels.Group);

        _stage.Children.Clear();
        _cards.Clear();

        var items = Items ?? Array.Empty<OrbitCardItem>();
        for (int i = 0; i < items.Count; i++)
        {
            var card = BuildCard(items[i], i, m);
            _cards.Add(card);
            _stage.Children.Add(card);
        }

        Relayout(false);
    }

    OrbitCard BuildCard(OrbitCardItem item, int index, (double cw, double ch, double h, double name, double body) m)
    {
        var card = new OrbitCard
        {
            Item = item,
            Width = m.cw,
            Height = m.ch,
            NameFontSize = m.name,
            BodyFontSize = m.body,
            InitialsFontSize = m.cw * 0.22,
            PaletteIndex = index,
            ShowDescription = Size != OrbitCardSize.Small,
            LinkLabel = Labels.Link,
            Content = ImageFactory?.Invoke(item, index),
        };

        card.PointerEntered += (_, _) => OnCardEnter(index);
        card.Tapped += (_, e) =>
        {
            Request(index, true);
            card.Focus(FocusState.Pointer);
            if (!IsFromButton(e.OriginalSource as DependencyObject, card)) RaiseItemInvoked(index);
        };
        card.GotFocus += (_, e) =>
        {
            if (ReferenceEquals(e.OriginalSource, card) || e.OriginalSource is DependencyObject d && IsInside(d, card))
                Request(index, true);
        };
        card.KeyDown += (_, e) => OnCardKeyDown(index, e);
        return card;
    }

    static bool IsFromButton(DependencyObject? node, DependencyObject card)
    {
        for (; node is not null && !ReferenceEquals(node, card); node = VisualTreeHelper.GetParent(node))
            if (node is ButtonBase) return true;
        return false;
    }

    void RaiseItemInvoked(int index)
    {
        if (index < _cards.Count && _cards[index].Item is { } item)
            ItemInvoked?.Invoke(this, new OrbitCardInvokedEventArgs(item, index));
    }

    static bool IsInside(DependencyObject node, DependencyObject ancestor)
    {
        for (var n = node; n is not null; n = VisualTreeHelper.GetParent(n))
            if (ReferenceEquals(n, ancestor)) return true;
        return false;
    }

    bool FocusInsideCards()
    {
        if (XamlRoot is null) return false;

        var f = FocusManager.GetFocusedElement(XamlRoot) as DependencyObject;
        for (; f is not null; f = VisualTreeHelper.GetParent(f))
            if (f is OrbitCard) return true;
        return false;
    }

    void OnCardEnter(int index)
    {
        _hovered = true;
        Request(IsOpen && _settling ? ActiveIndex : index, true);
    }

    void OnStagePointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (_stage is null) return;

        var p = e.GetCurrentPoint(_stage).Position;
        if (p.X >= 0 && p.Y >= 0 && p.X <= _stage.ActualWidth && p.Y <= _stage.ActualHeight) return;

        _hovered = false;
        if (!FocusInsideCards()) Request(ActiveIndex, false);
    }

    void OnCardKeyDown(int index, KeyRoutedEventArgs e)
    {
        int n = _cards.Count;
        int? target = e.Key switch
        {
            Windows.System.VirtualKey.Right or Windows.System.VirtualKey.Down => Math.Min(index + 1, n - 1),
            Windows.System.VirtualKey.Left or Windows.System.VirtualKey.Up => Math.Max(index - 1, 0),
            Windows.System.VirtualKey.Home => 0,
            Windows.System.VirtualKey.End => n - 1,
            _ => null,
        };

        if (target is int t)
        {
            Request(t, true);
            _cards[t].Focus(FocusState.Keyboard);
            e.Handled = true;
        }
        else if (e.Key == Windows.System.VirtualKey.Escape)
        {
            _hovered = false;
            Request(ActiveIndex, false);
            Focus(FocusState.Programmatic);
            e.Handled = true;
        }
        else if ((e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.Space) && ReferenceEquals(e.OriginalSource, _cards[index]))
        {
            RaiseItemInvoked(index);
            e.Handled = true;
        }
    }

    void Request(int active, bool open)
    {
        if (_cards.Count == 0) return;

        active = Math.Clamp(active, 0, _cards.Count - 1);
        bool activeChanged = active != ActiveIndex;
        bool openChanged = open != IsOpen;
        if (!activeChanged && !openChanged) return;

        if (openChanged) BeginSettle();

        bool changed = false;
        if (activeChanged)
        {
            if (Value is null)
            {
                _active = active;
                changed = true;
            }
            ValueChanged?.Invoke(this, active);
        }

        if (openChanged)
        {
            if (Open is null)
            {
                _open = open;
                changed = true;
            }
            OpenChanged?.Invoke(this, open);
        }

        if (changed) Relayout(true);
    }

    void OnOpenPropertyChanged()
    {
        BeginSettle();
        Relayout(true);
    }

    void Relayout(bool animate)
    {
        int n = _cards.Count;
        if (n == 0) return;

        animate &= IsAnimationEnabled;

        var m = Metrics;
        int a = ActiveIndex;
        bool open = IsOpen;
        double lift = Lift;

        double pad = 16;
        double step = Spread;
        if (n > 1 && _w > 0) step = Math.Max(8, Math.Min(Spread, (_w - 2 * pad - m.cw) / (n - 1)));

        int cols = 1, rows = 1;
        double gridScale = 1, gap = 12;
        if (open && Layout == OrbitCardLayout.Grid) (cols, rows, gridScale) = GridFit(n, m.cw, m.ch, gap);

        double k = m.cw / 150.0;
        double c = (n - 1) / 2.0;

        for (int i = 0; i < n; i++)
        {
            var card = _cards[i];
            OrbitPose pose;
            int z;

            if (!open)
            {
                int rel = i - a;
                double r = Math.Clamp(rel, -4, 4);
                double jy = rel == 0 ? 0 : ((i * 37) % 7 - 3) * 0.6 * k;
                double jr = rel == 0 ? 0 : ((i * 53) % 5 - 2) * 0.7;
                pose = new OrbitPose(r * 7 * k, Math.Abs(r) * 4 * k + jy, r * 3.2 + jr, 1 - 0.015 * Math.Abs(r));
                z = i == a ? n + 1 : n - Math.Abs(rel) * 2 - (rel > 0 ? 1 : 0);
            }
            else if (Layout == OrbitCardLayout.Grid)
            {
                int row = i / cols, col = i % cols;
                int inRow = row == rows - 1 ? n - row * cols : cols;
                double x = (col - (inRow - 1) / 2.0) * (m.cw + gap) * gridScale;
                double y = (row - (rows - 1) / 2.0) * (m.ch + gap) * gridScale;
                if (i == a) y -= lift;
                pose = new OrbitPose(x, y, 0, gridScale);
                z = i == a ? n + 1 : n - Math.Abs(i - a);
            }
            else
            {
                double d = i - c;
                double nrm = c > 0 ? d / c : 0;
                double y = Layout == OrbitCardLayout.Arc ? nrm * nrm * (m.ch * 0.12) : 0;
                double rot = Layout == OrbitCardLayout.Arc ? nrm * 12 : 0;
                if (i == a) y -= lift;
                pose = new OrbitPose(d * step, y, rot, 1);
                z = i == a ? n + 1 : n - Math.Abs(i - a);
            }

            Canvas.SetZIndex(card, z);
            card.IsTabStop = i == a;
            card.LinkTabStop = open && i == a;
            AutomationProperties.SetItemStatus(card, i == a ? "current" : string.Empty);
            Tween(card, pose, animate);
        }
    }

    (int cols, int rows, double scale) GridFit(int n, double cw, double ch, double gap)
    {
        double aw = _w > 0 ? Math.Max(1, _w - 32) : double.MaxValue;
        double ah = _h > 0 ? Math.Max(1, _h - 32) : double.MaxValue;
        int bestC = 1;
        double best = -1;

        for (int c = 1; c <= n; c++)
        {
            int r = (int)Math.Ceiling(n / (double)c);
            double s = Math.Min(1.0, Math.Min(aw / (c * cw + (c - 1) * gap), ah / (r * ch + (r - 1) * gap)));
            if (s > best + 1e-6)
            {
                best = s;
                bestC = c;
            }
        }

        return (bestC, (int)Math.Ceiling(n / (double)bestC), Math.Max(0.1, best));
    }

    static void Tween(OrbitCard card, OrbitPose to, bool animate)
    {
        var xf = card.PoseTransform;
        double fx = xf.TranslateX, fy = xf.TranslateY, fr = xf.Rotation, fs = xf.ScaleX;

        card.Running?.Stop();
        card.Running = null;

        xf.TranslateX = to.X;
        xf.TranslateY = to.Y;
        xf.Rotation = to.Rot;
        xf.ScaleX = to.Scale;
        xf.ScaleY = to.Scale;

        if (!animate) return;
        if (Math.Abs(fx - to.X) < 0.05 && Math.Abs(fy - to.Y) < 0.05 && Math.Abs(fr - to.Rot) < 0.05 && Math.Abs(fs - to.Scale) < 0.001) return;

        var sb = new Storyboard();
        Add(sb, xf, "TranslateX", fx, to.X);
        Add(sb, xf, "TranslateY", fy, to.Y);
        Add(sb, xf, "Rotation", fr, to.Rot);
        Add(sb, xf, "ScaleX", fs, to.Scale);
        Add(sb, xf, "ScaleY", fs, to.Scale);
        sb.Completed += (_, _) =>
        {
            if (ReferenceEquals(card.Running, sb)) card.Running = null;
        };
        card.Running = sb;
        sb.Begin();
    }

    static void Add(Storyboard sb, CompositeTransform target, string property, double from, double to)
    {
        var a = new DoubleAnimationUsingKeyFrames
        {
            EnableDependentAnimation = true,
            FillBehavior = FillBehavior.Stop,
        };
        a.KeyFrames.Add(new DiscreteDoubleKeyFrame { KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero), Value = from });
        a.KeyFrames.Add(new SplineDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TweenDuration),
            Value = to,
            KeySpline = new KeySpline { ControlPoint1 = new Point(0.2, 0.8), ControlPoint2 = new Point(0.2, 1.0) },
        });
        Storyboard.SetTarget(a, target);
        Storyboard.SetTargetProperty(a, property);
        sb.Children.Add(a);
    }
}
