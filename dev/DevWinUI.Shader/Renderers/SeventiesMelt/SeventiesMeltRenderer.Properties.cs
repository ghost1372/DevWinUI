using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class SeventiesMeltRenderer
{
    private float speed = 1f;
    private float warpSpeed = 1f;
    private float zoom = 40f;
    private float xAmplitude = 1f;
    private float yAmplitude = 1f;
    private float scaleAmplitude = 1.25f;
    private float vignetteStrength = 1f;

    public double Speed
    {
        get => speed;
        set => SetValue(SpeedProperty, value);
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(SeventiesMeltRenderer), new PropertyMetadata(1.0d, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SeventiesMeltRenderer)d;
        ctl.speed = (float)(double)e.NewValue;
    }

    public double WarpSpeed
    {
        get => warpSpeed;
        set => SetValue(WarpSpeedProperty, value);
    }

    public static readonly DependencyProperty WarpSpeedProperty =
        DependencyProperty.Register(nameof(WarpSpeed), typeof(double), typeof(SeventiesMeltRenderer), new PropertyMetadata(1.0, OnWarpSpeedChanged));

    private static void OnWarpSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SeventiesMeltRenderer)d;
        ctl.warpSpeed = (float)(double)e.NewValue;
    }

    public double Zoom
    {
        get => zoom;
        set => SetValue(ZoomProperty, value);
    }

    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(SeventiesMeltRenderer), new PropertyMetadata(40.0d, OnZoomChanged));

    private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SeventiesMeltRenderer)d;
        ctl.zoom = (float)(double)e.NewValue;
    }

    public double XAmplitude
    {
        get => xAmplitude;
        set => SetValue(XAmplitudeProperty, value);
    }

    public static readonly DependencyProperty XAmplitudeProperty =
        DependencyProperty.Register(nameof(XAmplitude), typeof(double), typeof(SeventiesMeltRenderer), new PropertyMetadata(1.0d, OnXAmplitudeChanged));

    private static void OnXAmplitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SeventiesMeltRenderer)d;
        ctl.xAmplitude = (float)(double)e.NewValue;
    }

    public double YAmplitude
    {
        get => yAmplitude;
        set => SetValue(YAmplitudeProperty, value);
    }

    public static readonly DependencyProperty YAmplitudeProperty =
        DependencyProperty.Register(nameof(YAmplitude), typeof(double), typeof(SeventiesMeltRenderer), new PropertyMetadata(1.0d, OnYAmplitudeChanged));

    private static void OnYAmplitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SeventiesMeltRenderer)d;
        ctl.yAmplitude = (float)(double)e.NewValue;
    }

    public double ScaleAmplitude
    {
        get => scaleAmplitude;
        set => SetValue(ScaleAmplitudeProperty, value);
    }

    public static readonly DependencyProperty ScaleAmplitudeProperty =
        DependencyProperty.Register(nameof(ScaleAmplitude), typeof(double), typeof(SeventiesMeltRenderer), new PropertyMetadata(1.25d, OnScaleAmplitudeChanged));

    private static void OnScaleAmplitudeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SeventiesMeltRenderer)d;
        ctl.scaleAmplitude = (float)(double)e.NewValue;
    }

    public double VignetteStrength
    {
        get => vignetteStrength;
        set => SetValue(VignetteStrengthProperty, value);
    }

    public static readonly DependencyProperty VignetteStrengthProperty =
        DependencyProperty.Register(nameof(VignetteStrength), typeof(double), typeof(SeventiesMeltRenderer), new PropertyMetadata(1.0d, OnVignetteStrengthChanged));

    private static void OnVignetteStrengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SeventiesMeltRenderer)d;
        ctl.vignetteStrength = (float)(double)e.NewValue;
    }
}
