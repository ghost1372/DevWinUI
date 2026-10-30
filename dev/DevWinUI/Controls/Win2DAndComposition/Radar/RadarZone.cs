namespace DevWinUI;

public sealed partial class RadarZone : RadarElement
{
    public RadarZoneShape Shape
    {
        get { return (RadarZoneShape)GetValue(ShapeProperty); }
        set { SetValue(ShapeProperty, value); }
    }

    public static readonly DependencyProperty ShapeProperty =
        DependencyProperty.Register(nameof(Shape), typeof(RadarZoneShape), typeof(RadarZone), new PropertyMetadata(RadarZoneShape.Circle, OnVisualChanged));

    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }

    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(RadarZone), new PropertyMetadata(0.0, OnVisualChanged));

    public double Distance
    {
        get { return (double)GetValue(DistanceProperty); }
        set { SetValue(DistanceProperty, value); }
    }

    public static readonly DependencyProperty DistanceProperty =
        DependencyProperty.Register(nameof(Distance), typeof(double), typeof(RadarZone), new PropertyMetadata(0.5, OnVisualChanged));

    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(nameof(Radius), typeof(double), typeof(RadarZone), new PropertyMetadata(0.2, OnVisualChanged));

    public PointCollection? Points
    {
        get { return (PointCollection?)GetValue(PointsProperty); }
        set { SetValue(PointsProperty, value); }
    }

    public static readonly DependencyProperty PointsProperty =
        DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(RadarZone), new PropertyMetadata(null, OnVisualChanged));

    public Brush? Foreground
    {
        get { return (Brush?)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }

    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(RadarZone), new PropertyMetadata(null, OnVisualChanged));

    public double FillOpacity
    {
        get { return (double)GetValue(FillOpacityProperty); }
        set { SetValue(FillOpacityProperty, value); }
    }

    public static readonly DependencyProperty FillOpacityProperty =
        DependencyProperty.Register(nameof(FillOpacity), typeof(double), typeof(RadarZone), new PropertyMetadata(0.12, OnVisualChanged));

    public bool Dashed
    {
        get { return (bool)GetValue(DashedProperty); }
        set { SetValue(DashedProperty, value); }
    }

    public static readonly DependencyProperty DashedProperty =
        DependencyProperty.Register(nameof(Dashed), typeof(bool), typeof(RadarZone), new PropertyMetadata(true, OnVisualChanged));

    public RadarVisibility Visibility
    {
        get { return (RadarVisibility)GetValue(VisibilityProperty); }
        set { SetValue(VisibilityProperty, value); }
    }

    public static readonly DependencyProperty VisibilityProperty =
        DependencyProperty.Register(nameof(Visibility), typeof(RadarVisibility), typeof(RadarZone), new PropertyMetadata(RadarVisibility.Always, OnVisualChanged));

    public string? Label
    {
        get { return (string?)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(RadarZone), new PropertyMetadata(null, OnVisualChanged));
}
