namespace DevWinUI;

public sealed partial class Diagram
{
    private const double HintMinSpace = 140;

    private Grid? _dock;
    private StackPanel?[]? _hosts;
    private readonly List<FrameworkElement> _watched = new();
    private XamlRoot? _watchedRoot;
    private int _settlePasses, _settleStable;
    private bool _settling;

    private static readonly string[] HostNames =
    {
        "PART_DockTopLeft", "PART_DockTopCenter", "PART_DockTopRight",
        "PART_DockBottomLeft", "PART_DockBottomCenter", "PART_DockBottomRight",
    };

    private void BindDock()
    {
        if (_dock != null) _dock.SizeChanged -= Dock_SizeChanged;

        _dock = GetTemplateChild("PART_Dock") as Grid;
        _hosts = new StackPanel?[HostNames.Length];
        for (int i = 0; i < HostNames.Length; i++) _hosts[i] = GetTemplateChild(HostNames[i]) as StackPanel;

        if (_dock != null) _dock.SizeChanged += Dock_SizeChanged;
        ApplyPlacements();
    }

    private void WatchViewport()
    {
        UnwatchViewport();

        _watchedRoot = XamlRoot;
        if (_watchedRoot != null) _watchedRoot.Changed += ViewportChanged;

        for (var o = VisualTreeHelper.GetParent(this); o != null; o = VisualTreeHelper.GetParent(o))
        {
            if (o is not FrameworkElement fe) continue;

            fe.SizeChanged += ViewportSizeChanged;
            if (fe is ScrollViewer sv) sv.ViewChanged += ViewportViewChanged;
            _watched.Add(fe);
        }

        UpdateDockViewport();
        StartSettle();
    }

    private void StartSettle()
    {
        _settlePasses = 0;
        _settleStable = 0;
        if (_settling) return;

        _settling = true;
        LayoutUpdated += Settle_LayoutUpdated;
    }

    private void StopSettle()
    {
        if (!_settling) return;

        _settling = false;
        LayoutUpdated -= Settle_LayoutUpdated;
    }

    private void Settle_LayoutUpdated(object? sender, object e)
    {
        _settlePasses++;
        _settleStable = UpdateDockViewport() ? 0 : _settleStable + 1;
        if (_settleStable >= 3 || _settlePasses > 120) StopSettle();
    }

    private void UnwatchViewport()
    {
        StopSettle();
        if (_watchedRoot != null) _watchedRoot.Changed -= ViewportChanged;
        _watchedRoot = null;

        foreach (var fe in _watched)
        {
            fe.SizeChanged -= ViewportSizeChanged;
            if (fe is ScrollViewer sv) sv.ViewChanged -= ViewportViewChanged;
        }

        _watched.Clear();
    }

    private void ViewportChanged(XamlRoot sender, XamlRootChangedEventArgs args) => UpdateDockViewport();

    private void ViewportSizeChanged(object sender, SizeChangedEventArgs e) => UpdateDockViewport();

    private void ViewportViewChanged(object? sender, ScrollViewerViewChangedEventArgs e) => UpdateDockViewport();

    private static Rect Intersect(Rect a, Rect b)
    {
        double l = Math.Max(a.Left, b.Left), t = Math.Max(a.Top, b.Top);
        double r = Math.Min(a.Right, b.Right), bt = Math.Min(a.Bottom, b.Bottom);
        return new Rect(l, t, Math.Max(0, r - l), Math.Max(0, bt - t));
    }

    private Rect ClipTo(Rect visible, UIElement? element, double width, double height)
    {
        if (element == null) return visible;

        try
        {
            var bounds = element.TransformToVisual(this).TransformBounds(new Rect(0, 0, width, height));
            return Intersect(visible, bounds);
        }
        catch
        {
            return visible;
        }
    }

    private bool UpdateDockViewport()
    {
        if (_dock == null || XamlRoot == null || ActualWidth <= 0 || ActualHeight <= 0) return true;

        var visible = new Rect(0, 0, ActualWidth, ActualHeight);
        visible = ClipTo(visible, XamlRoot.Content, XamlRoot.Size.Width, XamlRoot.Size.Height);

        for (var o = VisualTreeHelper.GetParent(this); o != null; o = VisualTreeHelper.GetParent(o))
        {
            if (o is ScrollViewer sv) visible = ClipTo(visible, sv, sv.ActualWidth, sv.ActualHeight);
        }

        if (visible.Width < 1 || visible.Height < 1) return true;

        var margin = new Thickness(
            Math.Max(0, visible.Left),
            Math.Max(0, visible.Top),
            Math.Max(0, ActualWidth - visible.Right),
            Math.Max(0, ActualHeight - visible.Bottom));

        var m = _dock.Margin;
        if (Math.Abs(m.Left - margin.Left) < 0.5 && Math.Abs(m.Top - margin.Top) < 0.5 &&
            Math.Abs(m.Right - margin.Right) < 0.5 && Math.Abs(m.Bottom - margin.Bottom) < 0.5) return false;

        _dock.Margin = margin;
        return true;
    }

    private void Dock_SizeChanged(object sender, SizeChangedEventArgs e) => UpdateHint();

    private void ApplyPlacements()
    {
        if (_hosts == null) return;

        foreach (var host in _hosts) host?.Children.Clear();

        Dock(_controls, ControlsPlacement);
        Dock(_mini, MinimapPlacement);
        Dock(_legend, LegendPlacement);
        Dock(_conflictPanel, ConflictsPlacement);
        Dock(_hint, HintPlacement);
        UpdateHint();
    }

    private void Dock(FrameworkElement? element, DiagramPanelPlacement placement)
    {
        if (element == null) return;

        if (element.Parent is Panel old) old.Children.Remove(element);

        int index = (int)placement;
        element.HorizontalAlignment = (index % 3) switch
        {
            0 => HorizontalAlignment.Left,
            1 => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Right,
        };
        element.VerticalAlignment = index < 3 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
        _hosts![index]?.Children.Add(element);
    }

    private bool HintHasSpace()
    {
        if (_dock == null || _dock.ActualWidth <= 0) return true;
        if ((int)HintPlacement % 3 != 1) return true;

        return _dock.ColumnDefinitions[1].ActualWidth >= HintMinSpace;
    }

    public DiagramPanelPlacement ControlsPlacement
    {
        get { return (DiagramPanelPlacement)GetValue(ControlsPlacementProperty); }
        set { SetValue(ControlsPlacementProperty, value); }
    }

    public static readonly DependencyProperty ControlsPlacementProperty =
        DependencyProperty.Register(nameof(ControlsPlacement), typeof(DiagramPanelPlacement), typeof(Diagram),
            new PropertyMetadata(DiagramPanelPlacement.BottomLeft, (d, e) => ((Diagram)d).ApplyPlacements()));

    public DiagramPanelPlacement MinimapPlacement
    {
        get { return (DiagramPanelPlacement)GetValue(MinimapPlacementProperty); }
        set { SetValue(MinimapPlacementProperty, value); }
    }

    public static readonly DependencyProperty MinimapPlacementProperty =
        DependencyProperty.Register(nameof(MinimapPlacement), typeof(DiagramPanelPlacement), typeof(Diagram),
            new PropertyMetadata(DiagramPanelPlacement.BottomRight, (d, e) => ((Diagram)d).ApplyPlacements()));

    public DiagramPanelPlacement HintPlacement
    {
        get { return (DiagramPanelPlacement)GetValue(HintPlacementProperty); }
        set { SetValue(HintPlacementProperty, value); }
    }

    public static readonly DependencyProperty HintPlacementProperty =
        DependencyProperty.Register(nameof(HintPlacement), typeof(DiagramPanelPlacement), typeof(Diagram),
            new PropertyMetadata(DiagramPanelPlacement.BottomCenter, (d, e) => ((Diagram)d).ApplyPlacements()));

    public DiagramPanelPlacement LegendPlacement
    {
        get { return (DiagramPanelPlacement)GetValue(LegendPlacementProperty); }
        set { SetValue(LegendPlacementProperty, value); }
    }

    public static readonly DependencyProperty LegendPlacementProperty =
        DependencyProperty.Register(nameof(LegendPlacement), typeof(DiagramPanelPlacement), typeof(Diagram),
            new PropertyMetadata(DiagramPanelPlacement.TopLeft, (d, e) => ((Diagram)d).ApplyPlacements()));

    public DiagramPanelPlacement ConflictsPlacement
    {
        get { return (DiagramPanelPlacement)GetValue(ConflictsPlacementProperty); }
        set { SetValue(ConflictsPlacementProperty, value); }
    }

    public static readonly DependencyProperty ConflictsPlacementProperty =
        DependencyProperty.Register(nameof(ConflictsPlacement), typeof(DiagramPanelPlacement), typeof(Diagram),
            new PropertyMetadata(DiagramPanelPlacement.TopRight, (d, e) => ((Diagram)d).ApplyPlacements()));
}
