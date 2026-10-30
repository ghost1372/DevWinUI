namespace DevWinUI;

public sealed partial class FlightStatusCard
{
    public string DepartureCode
    {
        get { return (string)GetValue(DepartureCodeProperty); }
        set { SetValue(DepartureCodeProperty, value); }
    }

    public static readonly DependencyProperty DepartureCodeProperty =
        DependencyProperty.Register(nameof(DepartureCode), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty, OnChanged));

    public string ArrivalCode
    {
        get { return (string)GetValue(ArrivalCodeProperty); }
        set { SetValue(ArrivalCodeProperty, value); }
    }

    public static readonly DependencyProperty ArrivalCodeProperty =
        DependencyProperty.Register(nameof(ArrivalCode), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty, OnChanged));

    public string DepartureCity
    {
        get { return (string)GetValue(DepartureCityProperty); }
        set { SetValue(DepartureCityProperty, value); }
    }

    public static readonly DependencyProperty DepartureCityProperty =
        DependencyProperty.Register(nameof(DepartureCity), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty, OnChanged));

    public string ArrivalCity
    {
        get { return (string)GetValue(ArrivalCityProperty); }
        set { SetValue(ArrivalCityProperty, value); }
    }

    public static readonly DependencyProperty ArrivalCityProperty =
        DependencyProperty.Register(nameof(ArrivalCity), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty, OnChanged));

    public string DepartureTime
    {
        get { return (string)GetValue(DepartureTimeProperty); }
        set { SetValue(DepartureTimeProperty, value); }
    }

    public static readonly DependencyProperty DepartureTimeProperty =
        DependencyProperty.Register(nameof(DepartureTime), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty));

    public string ArrivalTime
    {
        get { return (string)GetValue(ArrivalTimeProperty); }
        set { SetValue(ArrivalTimeProperty, value); }
    }

    public static readonly DependencyProperty ArrivalTimeProperty =
        DependencyProperty.Register(nameof(ArrivalTime), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty));

    public string Eta
    {
        get { return (string)GetValue(EtaProperty); }
        set { SetValue(EtaProperty, value); }
    }

    public static readonly DependencyProperty EtaProperty =
        DependencyProperty.Register(nameof(Eta), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty));

    public string Timezone
    {
        get { return (string)GetValue(TimezoneProperty); }
        set { SetValue(TimezoneProperty, value); }
    }

    public static readonly DependencyProperty TimezoneProperty =
        DependencyProperty.Register(nameof(Timezone), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty));

    public string NextEvent
    {
        get { return (string)GetValue(NextEventProperty); }
        set { SetValue(NextEventProperty, value); }
    }

    public static readonly DependencyProperty NextEventProperty =
        DependencyProperty.Register(nameof(NextEvent), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty));

    public string NextEventTime
    {
        get { return (string)GetValue(NextEventTimeProperty); }
        set { SetValue(NextEventTimeProperty, value); }
    }

    public static readonly DependencyProperty NextEventTimeProperty =
        DependencyProperty.Register(nameof(NextEventTime), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty));

    public string RemainingTime
    {
        get { return (string)GetValue(RemainingTimeProperty); }
        set { SetValue(RemainingTimeProperty, value); }
    }

    public static readonly DependencyProperty RemainingTimeProperty =
        DependencyProperty.Register(nameof(RemainingTime), typeof(string), typeof(FlightStatusCard), new PropertyMetadata(string.Empty));

    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(double), typeof(FlightStatusCard), new PropertyMetadata(0d, OnProgressChanged));

    public Brush ProgressBrush
    {
        get { return (Brush)GetValue(ProgressBrushProperty); }
        set { SetValue(ProgressBrushProperty, value); }
    }

    public static readonly DependencyProperty ProgressBrushProperty =
        DependencyProperty.Register(nameof(ProgressBrush), typeof(Brush), typeof(FlightStatusCard), new PropertyMetadata(null, OnModelChanged));

    public Brush GlowBrush
    {
        get { return (Brush)GetValue(GlowBrushProperty); }
        set { SetValue(GlowBrushProperty, value); }
    }

    public static readonly DependencyProperty GlowBrushProperty =
        DependencyProperty.Register(nameof(GlowBrush), typeof(Brush), typeof(FlightStatusCard), new PropertyMetadata(null, OnModelChanged));

    public Brush BadgeBrush
    {
        get { return (Brush)GetValue(BadgeBrushProperty); }
        set { SetValue(BadgeBrushProperty, value); }
    }

    public static readonly DependencyProperty BadgeBrushProperty =
        DependencyProperty.Register(nameof(BadgeBrush), typeof(Brush), typeof(FlightStatusCard), new PropertyMetadata(null, OnModelChanged));

    public Brush BadgeGlyphBrush
    {
        get { return (Brush)GetValue(BadgeGlyphBrushProperty); }
        set { SetValue(BadgeGlyphBrushProperty, value); }
    }

    public static readonly DependencyProperty BadgeGlyphBrushProperty =
        DependencyProperty.Register(nameof(BadgeGlyphBrush), typeof(Brush), typeof(FlightStatusCard), new PropertyMetadata(null, OnModelChanged));

    public FlightStatusCardResizeMode ResizeMode
    {
        get { return (FlightStatusCardResizeMode)GetValue(ResizeModeProperty); }
        set { SetValue(ResizeModeProperty, value); }
    }

    public static readonly DependencyProperty ResizeModeProperty =
        DependencyProperty.Register(nameof(ResizeMode), typeof(FlightStatusCardResizeMode), typeof(FlightStatusCard), new PropertyMetadata(FlightStatusCardResizeMode.Scale, OnResizeModeChanged));
}
