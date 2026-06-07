using Microsoft.UI.Xaml;
using Windows.UI;

namespace DevWinUI;

public partial class CosmicRenderer
{
    private float speed = 1f;
    private float rotationSpeed = 0.02f;
    private float frequency = 100f;
    private float hotspotContrast = 40f;
    private float hotspotStrength = 200f;
    private float edgeFalloff = 250f;
    private float exposure = 1f;
    private Color accentColor1 = Color.FromArgb(255, 25, 25, 51);
    private Color accentColor2 = Color.FromArgb(255, 51, 102, 255);
    private Color accentColor3 = Color.FromArgb(255, 204, 51, 255);
    private Color accentColor4 = Color.FromArgb(255, 255, 255, 255);
    public double Speed
    {
        get => speed;
        set => SetValue(SpeedProperty, value);
    }
    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(CosmicRenderer), new PropertyMetadata(1.0d, OnSpeedChanged));
    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.speed = (float)(double)e.NewValue;
    }

    public double RotationSpeed
    {
        get => rotationSpeed;
        set => SetValue(RotationSpeedProperty, value);
    }
    public static readonly DependencyProperty RotationSpeedProperty =
        DependencyProperty.Register(nameof(RotationSpeed), typeof(double), typeof(CosmicRenderer), new PropertyMetadata(0.02d, OnRotationSpeedChanged));
    private static void OnRotationSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.rotationSpeed = (float)(double)e.NewValue;
    }

    public double Frequency
    {
        get => frequency;
        set => SetValue(FrequencyProperty, value);
    }
    public static readonly DependencyProperty FrequencyProperty =
        DependencyProperty.Register(nameof(Frequency), typeof(double), typeof(CosmicRenderer), new PropertyMetadata(100.0d, OnFrequencyChanged));
    private static void OnFrequencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.frequency = (float)(double)e.NewValue;
    }

    public double HotspotContrast
    {
        get => hotspotContrast;
        set => SetValue(HotspotContrastProperty, value);
    }
    public static readonly DependencyProperty HotspotContrastProperty =
        DependencyProperty.Register(nameof(HotspotContrast), typeof(double), typeof(CosmicRenderer), new PropertyMetadata(40.0d, OnHotspotContrastChanged));
    private static void OnHotspotContrastChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.hotspotContrast = (float)(double)e.NewValue;
    }

    public double HotspotStrength
    {
        get => hotspotStrength;
        set => SetValue(HotspotStrengthProperty, value);
    }
    public static readonly DependencyProperty HotspotStrengthProperty =
        DependencyProperty.Register(nameof(HotspotStrength), typeof(double), typeof(CosmicRenderer), new PropertyMetadata(200.0, OnHotspotStrengthChanged));
    private static void OnHotspotStrengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.hotspotStrength = (float)(double)e.NewValue;
    }

    public double EdgeFalloff
    {
        get => edgeFalloff;
        set => SetValue(EdgeFalloffProperty, value);
    }
    public static readonly DependencyProperty EdgeFalloffProperty =
        DependencyProperty.Register(nameof(EdgeFalloff), typeof(double), typeof(CosmicRenderer), new PropertyMetadata(250.0, OnEdgeFalloffChanged));
    private static void OnEdgeFalloffChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.edgeFalloff = (float)(double)e.NewValue;
    }

    public double Exposure
    {
        get => exposure;
        set => SetValue(ExposureProperty, value);
    }
    public static readonly DependencyProperty ExposureProperty =
        DependencyProperty.Register(nameof(Exposure), typeof(double), typeof(CosmicRenderer), new PropertyMetadata(1.0d, OnExposureChanged));
    private static void OnExposureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.exposure = (float)(double)e.NewValue;
    }

    public Color AccentColor1
    {
        get => accentColor1;
        set => SetValue(AccentColor1Property, value);
    }
    public static readonly DependencyProperty AccentColor1Property =
        DependencyProperty.Register(nameof(AccentColor1), typeof(Color), typeof(CosmicRenderer), new PropertyMetadata(Color.FromArgb(255, 25, 25, 51), OnAccentColor1Changed));
    private static void OnAccentColor1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.accentColor1 = (Color)e.NewValue;
    }

    public Color AccentColor2
    {
        get => accentColor2;
        set => SetValue(AccentColor2Property, value);
    }
    public static readonly DependencyProperty AccentColor2Property =
        DependencyProperty.Register(nameof(AccentColor2), typeof(Color), typeof(CosmicRenderer), new PropertyMetadata(Color.FromArgb(255, 51, 102, 255), OnAccentColor2Changed));
    private static void OnAccentColor2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.accentColor2 = (Color)e.NewValue;
    }

    public Color AccentColor3
    {
        get => accentColor3;
        set => SetValue(AccentColor3Property, value);
    }
    public static readonly DependencyProperty AccentColor3Property =
        DependencyProperty.Register(nameof(AccentColor3), typeof(Color), typeof(CosmicRenderer), new PropertyMetadata(Color.FromArgb(255, 204, 51, 255), OnAccentColor3Changed));
    private static void OnAccentColor3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.accentColor3 = (Color)e.NewValue;
    }

    public Color AccentColor4
    {
        get => accentColor4;
        set => SetValue(AccentColor4Property, value);
    }
    public static readonly DependencyProperty AccentColor4Property =
        DependencyProperty.Register(nameof(AccentColor4), typeof(Color), typeof(CosmicRenderer), new PropertyMetadata(Color.FromArgb(255, 255, 255, 255), OnAccentColor4Changed));
    private static void OnAccentColor4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CosmicRenderer)d;
        ctl.accentColor4 = (Color)e.NewValue;
    }
}
