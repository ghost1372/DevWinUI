using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.UI.Dispatching;

namespace DevWinUI;

[TemplatePart(Name = CanvasPartName, Type = typeof(CanvasControl))]
[TemplatePart(Name = LanesHostPartName, Type = typeof(ItemsControl))]
public partial class Roadmap : ItemsControl
{
    private const string CanvasPartName = "PART_Canvas";
    private const string LanesHostPartName = "PART_LanesHost";

    internal const double EndDotRadius = 7;
    internal const double ContentGap = 8;
    internal const double StemInset = 8;
    internal const double AxisInset = 12;
    internal const double ItemGap = 16;
    internal const double VerticalMargin = 4;

    private CanvasControl? _canvas;
    private ItemsControl? _lanesHost;
    private DispatcherQueueTimer? _animationTimer;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private bool _isLoaded;
    private double _bootStart;
    private bool _booting;

    internal RoadmapPanel? HostPanel { get; set; }

    public Roadmap()
    {
        DefaultStyleKey = typeof(Roadmap);
        SetValue(LanesProperty, new ObservableCollection<RoadmapLane>());

        RegisterPropertyChangedCallback(ForegroundProperty, (s, dp) => InvalidateCanvas());
        RegisterPropertyChangedCallback(FontFamilyProperty, (s, dp) => InvalidateCanvas());

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        DetachCanvas();
        _canvas = GetTemplateChild(CanvasPartName) as CanvasControl;
        _lanesHost = GetTemplateChild(LanesHostPartName) as ItemsControl;
        AttachCanvas();
        UpdateLanesVisibility();
    }

    #region Containers

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is RoadmapItem;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new RoadmapItem();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is RoadmapItem container)
        {
            container.Mode = Mode;
        }
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        InvalidateCanvas();
        UpdateAnimation();
    }

    #endregion

    #region Canvas

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = true;
        AttachCanvas();
        InvalidateCanvas();

        if (IsEnabled(BootAnimation, false))
        {
            _bootStart = _clock.Elapsed.TotalSeconds;
            _booting = true;
            ApplyBootOpacity();
        }

        UpdateAnimation();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = false;
        _booting = false;
        ApplyBootOpacity();
        UpdateAnimation();
        DetachCanvas();
    }

    private void AttachCanvas()
    {
        if (_canvas != null)
        {
            _canvas.Draw -= OnCanvasDraw;
            _canvas.Draw += OnCanvasDraw;
        }
    }

    private void DetachCanvas()
    {
        if (_canvas != null)
        {
            _canvas.Draw -= OnCanvasDraw;
        }
    }

    internal void InvalidateCanvas()
    {
        _canvas?.Invalidate();
    }

    internal void InvalidateLayoutAndCanvas()
    {
        HostPanel?.InvalidateMeasure();
        _canvas?.Invalidate();
    }

    private void ApplyModeToContainers()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (ContainerFromIndex(i) is RoadmapItem container)
            {
                container.Mode = Mode;
            }
        }
    }

    #endregion

    #region Animation

    public void PlayBootAnimation()
    {
        _bootStart = _clock.Elapsed.TotalSeconds;
        _booting = true;
        ApplyBootOpacity();
        UpdateAnimation();
    }

    internal bool IsEnabled(RoadmapToggle toggle, bool autoValue)
    {
        return toggle == RoadmapToggle.On || (toggle == RoadmapToggle.Auto && autoValue);
    }

    internal bool ScanlinesEnabled
    {
        get { return IsEnabled(Scanlines, Mode == RoadmapMode.Neon); }
    }

    internal bool GridEnabled
    {
        get { return IsEnabled(GridBackdrop, Mode == RoadmapMode.Neon); }
    }

    internal bool AxisFlowEnabled
    {
        get { return IsEnabled(AxisFlow, Mode == RoadmapMode.Neon); }
    }

    internal bool ScanBeamEnabled
    {
        get { return IsEnabled(ScanBeam, false); }
    }

    internal bool FlickerEnabled
    {
        get { return IsEnabled(Flicker, false); }
    }

    internal double ResolvedGlowRadius
    {
        get
        {
            if (double.IsNaN(GlowRadius))
            {
                return Mode == RoadmapMode.Neon ? 6 : 0;
            }

            return Math.Max(0, GlowRadius);
        }
    }

    internal float GetBootProgress()
    {
        if (!_booting)
        {
            return -1f;
        }

        double t = (_clock.Elapsed.TotalSeconds - _bootStart) / Math.Max(0.2, BootDuration);
        if (t >= 1)
        {
            return 1f;
        }

        return (float)(1 - Math.Pow(1 - Math.Max(0, t), 3));
    }

    internal void UpdateAnimation()
    {
        bool needed = NeedsAnimation();

        if (needed && _animationTimer is null)
        {
            _animationTimer = DispatcherQueue.CreateTimer();
            _animationTimer.Interval = TimeSpan.FromMilliseconds(33);
            _animationTimer.Tick += OnAnimationTick;
            _animationTimer.Start();
        }
        else if (!needed && _animationTimer != null)
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= OnAnimationTick;
            _animationTimer = null;
            InvalidateCanvas();
        }
    }

    private bool NeedsAnimation()
    {
        return _isLoaded
            && (_booting || AxisFlowEnabled || ScanBeamEnabled || FlickerEnabled || (AnimateActive && HasActiveItem()));
    }

    private void OnAnimationTick(DispatcherQueueTimer sender, object args)
    {
        if (_booting)
        {
            if (GetBootProgress() >= 1f)
            {
                _booting = false;
            }

            ApplyBootOpacity();

            if (!_booting)
            {
                UpdateAnimation();
            }
        }

        _canvas?.Invalidate();
    }

    private void ApplyBootOpacity()
    {
        RoadmapPanel? panel = HostPanel;
        if (panel is null)
        {
            return;
        }

        float progress = GetBootProgress();
        double head = panel.AxisStartX + progress * (panel.AxisEndX - panel.AxisStartX);

        foreach (UIElement child in panel.Children)
        {
            if (child is RoadmapItem item)
            {
                item.Opacity = progress < 0f ? 1 : Math.Clamp((head - item.LayoutCenterX + 30) / 60, 0, 1);
            }
        }
    }

    private bool HasActiveItem()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            RoadmapItem? item = Items[i] as RoadmapItem ?? ContainerFromIndex(i) as RoadmapItem;
            if (item?.Status == RoadmapItemStatus.Active)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Lanes

    private void OnLanesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (RoadmapLane lane in e.NewItems)
            {
                BindLane(lane);
            }
        }

        UpdateLanesVisibility();
    }

    private void BindLane(RoadmapLane lane)
    {
        if (lane.ReadLocalValue(RoadmapLane.HeaderWidthProperty) == DependencyProperty.UnsetValue)
        {
            lane.SetBinding(RoadmapLane.HeaderWidthProperty, new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(LaneHeaderWidth))
            });
        }
    }

    private void UpdateLanesVisibility()
    {
        if (_lanesHost != null)
        {
            _lanesHost.Visibility = Lanes != null && Lanes.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    #endregion

    #region Metrics

    internal RoadmapMetrics GetMetrics()
    {
        double itemWidth, marker, axis, stemThickness, stemLength, labelSize, labelOffset;
        RoadmapAxisCap start, end;

        switch (Mode)
        {
            case RoadmapMode.Boxed:
                itemWidth = 170; marker = 22; axis = 5; stemThickness = 1.5; stemLength = 72;
                labelSize = 15; labelOffset = 38; start = RoadmapAxisCap.None; end = RoadmapAxisCap.Arrow;
                break;

            case RoadmapMode.Icon:
                itemWidth = 150; marker = 64; axis = 2; stemThickness = 2; stemLength = 44;
                labelSize = 14; labelOffset = 0; start = RoadmapAxisCap.Dot; end = RoadmapAxisCap.Arrow;
                break;

            case RoadmapMode.Neon:
                itemWidth = 170; marker = 20; axis = 2; stemThickness = 1.5; stemLength = 72;
                labelSize = 14; labelOffset = 34; start = RoadmapAxisCap.Dot; end = RoadmapAxisCap.Arrow;
                break;

            default:
                itemWidth = 150; marker = 14; axis = 2.5; stemThickness = 1; stemLength = 96;
                labelSize = 26; labelOffset = 44; start = RoadmapAxisCap.Dot; end = RoadmapAxisCap.Dot;
                break;
        }

        return new RoadmapMetrics
        {
            ItemWidth = Resolve(ItemWidth, itemWidth),
            MarkerSize = Resolve(MarkerSize, marker),
            AxisThickness = Resolve(AxisThickness, axis),
            StemThickness = stemThickness,
            StemLength = Resolve(StemLength, stemLength),
            LabelFontSize = Resolve(LabelFontSize, labelSize),
            LabelOffset = labelOffset,
            StartCap = StartCap == RoadmapAxisCap.Auto ? start : StartCap,
            EndCap = EndCap == RoadmapAxisCap.Auto ? end : EndCap
        };
    }

    private static double Resolve(double value, double fallback)
    {
        return double.IsNaN(value) || value <= 0 ? fallback : value;
    }

    #endregion
}
