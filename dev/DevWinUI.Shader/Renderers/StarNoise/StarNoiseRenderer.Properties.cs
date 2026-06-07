using Microsoft.UI.Xaml;
using System.Numerics;
using Windows.UI;

namespace DevWinUI;

public partial class StarNoiseRenderer
{
    private float starDensity = 200f;
    private float starExposure = 200f;
    private float starThreshold = 8f;
    private float flickerSpeed = 1f;

    private Color color = Color.FromArgb(255, 255, 255, 255);

    public double StarDensity
    {
        get => starDensity;
        set => SetValue(StarDensityProperty, value);
    }

    public static readonly DependencyProperty StarDensityProperty =
        DependencyProperty.Register(nameof(StarDensity), typeof(double), typeof(StarNoiseRenderer), new PropertyMetadata(200.0d, OnStarDensityChanged));

    private static void OnStarDensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNoiseRenderer)d;
        ctl.starDensity = (float)(double)e.NewValue;
    }

    public double StarExposure
    {
        get => starExposure;
        set => SetValue(StarExposureProperty, value);
    }

    public static readonly DependencyProperty StarExposureProperty =
        DependencyProperty.Register(nameof(StarExposure), typeof(double), typeof(StarNoiseRenderer), new PropertyMetadata(200.0d, OnStarExposureChanged));

    private static void OnStarExposureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNoiseRenderer)d;
        ctl.starExposure = (float)(double)e.NewValue;
    }

    public double StarThreshold
    {
        get => starThreshold;
        set => SetValue(StarThresholdProperty, value);
    }

    public static readonly DependencyProperty StarThresholdProperty =
        DependencyProperty.Register(nameof(StarThreshold), typeof(double), typeof(StarNoiseRenderer), new PropertyMetadata(8.0d, OnStarThresholdChanged));

    private static void OnStarThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNoiseRenderer)d;
        ctl.starThreshold = (float)(double)e.NewValue;
    }

    public double FlickerSpeed
    {
        get => flickerSpeed;
        set => SetValue(FlickerSpeedProperty, value);
    }

    public static readonly DependencyProperty FlickerSpeedProperty =
        DependencyProperty.Register(nameof(FlickerSpeed), typeof(double), typeof(StarNoiseRenderer), new PropertyMetadata(1.0d, OnFlickerSpeedChanged));

    private static void OnFlickerSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNoiseRenderer)d;
        ctl.flickerSpeed = (float)(double)e.NewValue;
    }

    public Color Color
    {
        get => color;
        set => SetValue(ColorProperty, value);
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Vector3), typeof(StarNoiseRenderer), new PropertyMetadata(Color.FromArgb(255, 255, 255, 255), OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNoiseRenderer)d;
        ctl.color = (Color)e.NewValue;
    }
}
