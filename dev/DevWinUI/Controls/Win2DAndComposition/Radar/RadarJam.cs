namespace DevWinUI;

public sealed partial class RadarJam : RadarElement
{
    public double StartAngle
    {
        get { return (double)GetValue(StartAngleProperty); }
        set { SetValue(StartAngleProperty, value); }
    }

    public static readonly DependencyProperty StartAngleProperty =
        DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(RadarJam), new PropertyMetadata(0.0, OnVisualChanged));

    public double EndAngle
    {
        get { return (double)GetValue(EndAngleProperty); }
        set { SetValue(EndAngleProperty, value); }
    }

    public static readonly DependencyProperty EndAngleProperty =
        DependencyProperty.Register(nameof(EndAngle), typeof(double), typeof(RadarJam), new PropertyMetadata(60.0, OnVisualChanged));

    public double Intensity
    {
        get { return (double)GetValue(IntensityProperty); }
        set { SetValue(IntensityProperty, value); }
    }

    public static readonly DependencyProperty IntensityProperty =
        DependencyProperty.Register(nameof(Intensity), typeof(double), typeof(RadarJam), new PropertyMetadata(0.8, OnVisualChanged));

    public Brush? Foreground
    {
        get { return (Brush?)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }

    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(RadarJam), new PropertyMetadata(null, OnVisualChanged));
}
