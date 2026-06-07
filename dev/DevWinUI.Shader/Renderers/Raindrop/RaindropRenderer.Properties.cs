using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class RaindropRenderer
{
    private double raindropSpeed = 100.0;
    private double raindropSize = 100.0;
    private double raindropDensity = 40.0;
    private double raindropLightAngle = 135.0;
    private double raindropShadowIntensity = 0.0;

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(RaindropRenderer), new PropertyMetadata(100.0, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RaindropRenderer)d;
        ctl.raindropSpeed = (double)e.NewValue;
    }

    public double Size
    {
        get { return (double)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(RaindropRenderer), new PropertyMetadata(100.0, OnSizeChanged));

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RaindropRenderer)d;
        ctl.raindropSize = (double)e.NewValue;
    }

    public double Density
    {
        get { return (double)GetValue(DensityProperty); }
        set { SetValue(DensityProperty, value); }
    }

    public static readonly DependencyProperty DensityProperty =
        DependencyProperty.Register(nameof(Density), typeof(double), typeof(RaindropRenderer), new PropertyMetadata(40.0, OnDensityChanged));

    private static void OnDensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RaindropRenderer)d;
        ctl.raindropDensity = (double)e.NewValue;
    }

    public double LightAngle
    {
        get { return (double)GetValue(LightAngleProperty); }
        set { SetValue(LightAngleProperty, value); }
    }

    public static readonly DependencyProperty LightAngleProperty =
        DependencyProperty.Register(nameof(LightAngle), typeof(double), typeof(RaindropRenderer), new PropertyMetadata(135.0, OnLightAngleChanged));

    private static void OnLightAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RaindropRenderer)d;
        ctl.raindropLightAngle = (double)e.NewValue;
    }

    public double ShadowIntensity
    {
        get { return (double)GetValue(ShadowIntensityProperty); }
        set { SetValue(ShadowIntensityProperty, value); }
    }

    public static readonly DependencyProperty ShadowIntensityProperty =
        DependencyProperty.Register(nameof(ShadowIntensity), typeof(double), typeof(RaindropRenderer), new PropertyMetadata(0.0, OnShadowIntensityChanged));

    private static void OnShadowIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RaindropRenderer)d;
        ctl.raindropShadowIntensity = (double)e.NewValue;
    }
}
