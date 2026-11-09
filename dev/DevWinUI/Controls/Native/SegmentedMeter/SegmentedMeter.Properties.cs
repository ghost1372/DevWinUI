using System.Collections.ObjectModel;

namespace DevWinUI;

public sealed partial class SegmentedMeter
{
    public ObservableCollection<MeterSegment> Segments
    {
        get { return _segments; }
    }

    public double Minimum
    {
        get { return (double)GetValue(MinimumProperty); }
        set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(SegmentedMeter), new PropertyMetadata(0.0, OnVisualChanged));

    public double Maximum
    {
        get { return (double)GetValue(MaximumProperty); }
        set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(SegmentedMeter), new PropertyMetadata(100.0, OnVisualChanged));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SegmentedMeter), new PropertyMetadata(Orientation.Horizontal, OnVisualChanged));

    public MeterBarSize BarSize
    {
        get { return (MeterBarSize)GetValue(BarSizeProperty); }
        set { SetValue(BarSizeProperty, value); }
    }

    public static readonly DependencyProperty BarSizeProperty =
        DependencyProperty.Register(nameof(BarSize), typeof(MeterBarSize), typeof(SegmentedMeter), new PropertyMetadata(MeterBarSize.Default, OnVisualChanged));

    public double BarThickness
    {
        get { return (double)GetValue(BarThicknessProperty); }
        set { SetValue(BarThicknessProperty, value); }
    }

    public static readonly DependencyProperty BarThicknessProperty =
        DependencyProperty.Register(nameof(BarThickness), typeof(double), typeof(SegmentedMeter), new PropertyMetadata(double.NaN, OnVisualChanged));

    public double Gap
    {
        get { return (double)GetValue(GapProperty); }
        set { SetValue(GapProperty, value); }
    }

    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(SegmentedMeter), new PropertyMetadata(2.0, OnVisualChanged));

    public bool ShowLegend
    {
        get { return (bool)GetValue(ShowLegendProperty); }
        set { SetValue(ShowLegendProperty, value); }
    }

    public static readonly DependencyProperty ShowLegendProperty =
        DependencyProperty.Register(nameof(ShowLegend), typeof(bool), typeof(SegmentedMeter), new PropertyMetadata(true, OnVisualChanged));

    public bool ShowValue
    {
        get { return (bool)GetValue(ShowValueProperty); }
        set { SetValue(ShowValueProperty, value); }
    }

    public static readonly DependencyProperty ShowValueProperty =
        DependencyProperty.Register(nameof(ShowValue), typeof(bool), typeof(SegmentedMeter), new PropertyMetadata(true, OnVisualChanged));

    public MeterLegendPosition LegendPosition
    {
        get { return (MeterLegendPosition)GetValue(LegendPositionProperty); }
        set { SetValue(LegendPositionProperty, value); }
    }

    public static readonly DependencyProperty LegendPositionProperty =
        DependencyProperty.Register(nameof(LegendPosition), typeof(MeterLegendPosition), typeof(SegmentedMeter), new PropertyMetadata(MeterLegendPosition.End, OnVisualChanged));

    public Orientation LegendOrientation
    {
        get { return (Orientation)GetValue(LegendOrientationProperty); }
        set { SetValue(LegendOrientationProperty, value); }
    }

    public static readonly DependencyProperty LegendOrientationProperty =
        DependencyProperty.Register(nameof(LegendOrientation), typeof(Orientation), typeof(SegmentedMeter), new PropertyMetadata(Orientation.Horizontal, OnVisualChanged));

    public bool Animated
    {
        get { return (bool)GetValue(AnimatedProperty); }
        set { SetValue(AnimatedProperty, value); }
    }

    public static readonly DependencyProperty AnimatedProperty =
        DependencyProperty.Register(nameof(Animated), typeof(bool), typeof(SegmentedMeter), new PropertyMetadata(true));

    public object? StartContent
    {
        get { return GetValue(StartContentProperty); }
        set { SetValue(StartContentProperty, value); }
    }

    public static readonly DependencyProperty StartContentProperty =
        DependencyProperty.Register(nameof(StartContent), typeof(object), typeof(SegmentedMeter), new PropertyMetadata(null, OnSlotChanged));

    public object? EndContent
    {
        get { return GetValue(EndContentProperty); }
        set { SetValue(EndContentProperty, value); }
    }

    public static readonly DependencyProperty EndContentProperty =
        DependencyProperty.Register(nameof(EndContent), typeof(object), typeof(SegmentedMeter), new PropertyMetadata(null, OnSlotChanged));

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SegmentedMeter)d).RequestRebuild();
    }

    private static void OnSlotChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SegmentedMeter)d).UpdateSlots();
    }
}
