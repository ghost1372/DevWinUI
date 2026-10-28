namespace DevWinUI;

public sealed partial class LineSidebar
{
    public IList<string>? Items
    {
        get { return (IList<string>?)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(IList<string>), typeof(LineSidebar), new PropertyMetadata(null, OnRebuildChanged));

    public int ActiveIndex
    {
        get { return (int)GetValue(ActiveIndexProperty); }
        set { SetValue(ActiveIndexProperty, value); }
    }

    public static readonly DependencyProperty ActiveIndexProperty =
        DependencyProperty.Register(nameof(ActiveIndex), typeof(int), typeof(LineSidebar), new PropertyMetadata(0, OnActiveIndexChanged));

    public bool ShowMarker
    {
        get { return (bool)GetValue(ShowMarkerProperty); }
        set { SetValue(ShowMarkerProperty, value); }
    }

    public static readonly DependencyProperty ShowMarkerProperty =
        DependencyProperty.Register(nameof(ShowMarker), typeof(bool), typeof(LineSidebar), new PropertyMetadata(true, OnRebuildChanged));

    public bool ShowIndex
    {
        get { return (bool)GetValue(ShowIndexProperty); }
        set { SetValue(ShowIndexProperty, value); }
    }

    public static readonly DependencyProperty ShowIndexProperty =
        DependencyProperty.Register(nameof(ShowIndex), typeof(bool), typeof(LineSidebar), new PropertyMetadata(true, OnRebuildChanged));

    public bool ScaleTick
    {
        get { return (bool)GetValue(ScaleTickProperty); }
        set { SetValue(ScaleTickProperty, value); }
    }

    public static readonly DependencyProperty ScaleTickProperty =
        DependencyProperty.Register(nameof(ScaleTick), typeof(bool), typeof(LineSidebar), new PropertyMetadata(true));

    public double ItemGap
    {
        get { return (double)GetValue(ItemGapProperty); }
        set { SetValue(ItemGapProperty, value); }
    }

    public static readonly DependencyProperty ItemGapProperty =
        DependencyProperty.Register(nameof(ItemGap), typeof(double), typeof(LineSidebar), new PropertyMetadata(14d, OnRebuildChanged));

    public double MarkerLength
    {
        get { return (double)GetValue(MarkerLengthProperty); }
        set { SetValue(MarkerLengthProperty, value); }
    }

    public static readonly DependencyProperty MarkerLengthProperty =
        DependencyProperty.Register(nameof(MarkerLength), typeof(double), typeof(LineSidebar), new PropertyMetadata(28d, OnRebuildChanged));

    public double MarkerGap
    {
        get { return (double)GetValue(MarkerGapProperty); }
        set { SetValue(MarkerGapProperty, value); }
    }

    public static readonly DependencyProperty MarkerGapProperty =
        DependencyProperty.Register(nameof(MarkerGap), typeof(double), typeof(LineSidebar), new PropertyMetadata(14d, OnRebuildChanged));

    public double TickScale
    {
        get { return (double)GetValue(TickScaleProperty); }
        set { SetValue(TickScaleProperty, value); }
    }

    public static readonly DependencyProperty TickScaleProperty =
        DependencyProperty.Register(nameof(TickScale), typeof(double), typeof(LineSidebar), new PropertyMetadata(0.5, OnRebuildChanged));

    public double MaxShift
    {
        get { return (double)GetValue(MaxShiftProperty); }
        set { SetValue(MaxShiftProperty, value); }
    }

    public static readonly DependencyProperty MaxShiftProperty =
        DependencyProperty.Register(nameof(MaxShift), typeof(double), typeof(LineSidebar), new PropertyMetadata(12d));

    public double ProximityRadius
    {
        get { return (double)GetValue(ProximityRadiusProperty); }
        set { SetValue(ProximityRadiusProperty, value); }
    }

    public static readonly DependencyProperty ProximityRadiusProperty =
        DependencyProperty.Register(nameof(ProximityRadius), typeof(double), typeof(LineSidebar), new PropertyMetadata(120d));

    public double Smoothing
    {
        get { return (double)GetValue(SmoothingProperty); }
        set { SetValue(SmoothingProperty, value); }
    }

    public static readonly DependencyProperty SmoothingProperty =
        DependencyProperty.Register(nameof(Smoothing), typeof(double), typeof(LineSidebar), new PropertyMetadata(80d));

    public LineSidebarFalloff Falloff
    {
        get { return (LineSidebarFalloff)GetValue(FalloffProperty); }
        set { SetValue(FalloffProperty, value); }
    }

    public static readonly DependencyProperty FalloffProperty =
        DependencyProperty.Register(nameof(Falloff), typeof(LineSidebarFalloff), typeof(LineSidebar), new PropertyMetadata(LineSidebarFalloff.Smooth));

    public Color MarkerColor
    {
        get { return (Color)GetValue(MarkerColorProperty); }
        set { SetValue(MarkerColorProperty, value); }
    }

    public static readonly DependencyProperty MarkerColorProperty =
        DependencyProperty.Register(nameof(MarkerColor), typeof(Color), typeof(LineSidebar), new PropertyMetadata(Colors.Gray, OnRebuildChanged));

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    public static readonly DependencyProperty TextColorProperty =
        DependencyProperty.Register(nameof(TextColor), typeof(Color), typeof(LineSidebar), new PropertyMetadata(Colors.Gray, OnRebuildChanged));

    public Color AccentColor
    {
        get { return (Color)GetValue(AccentColorProperty); }
        set { SetValue(AccentColorProperty, value); }
    }

    public static readonly DependencyProperty AccentColorProperty =
        DependencyProperty.Register(nameof(AccentColor), typeof(Color), typeof(LineSidebar), new PropertyMetadata(Colors.White));
}
