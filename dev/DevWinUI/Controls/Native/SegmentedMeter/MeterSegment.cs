namespace DevWinUI;

public sealed partial class MeterSegment : DependencyObject
{
    internal event EventHandler? Changed;

    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(MeterSegment), new PropertyMetadata(0.0, OnSegmentChanged));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(MeterSegment), new PropertyMetadata(string.Empty, OnSegmentChanged));

    public MeterSegmentKind Kind
    {
        get { return (MeterSegmentKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(MeterSegmentKind), typeof(MeterSegment), new PropertyMetadata(MeterSegmentKind.Auto, OnSegmentChanged));

    public Brush? Fill
    {
        get { return (Brush?)GetValue(FillProperty); }
        set { SetValue(FillProperty, value); }
    }

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(MeterSegment), new PropertyMetadata(null, OnSegmentChanged));

    public string? Icon
    {
        get { return (string?)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(MeterSegment), new PropertyMetadata(null, OnSegmentChanged));

    private static void OnSegmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MeterSegment)d).Changed?.Invoke(d, EventArgs.Empty);
    }
}
