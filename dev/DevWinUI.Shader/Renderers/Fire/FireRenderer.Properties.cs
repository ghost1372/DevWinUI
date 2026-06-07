using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class FireRenderer
{
    private float speed = 1f;
    private float smokeSpeed = 1f;
    private float brightness = 1f;
    private float particleSize = 1f;
    private float particleDensity = 15f;
    private float smokeAmount = 1f;
    private float flameHeight = 1f;
    private float vignetteStrength = 1f;

    public double Speed
    {
        get => speed;
        set => SetValue(SpeedProperty, value);
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(FireRenderer), new PropertyMetadata(1.0d, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.speed = (float)(double)e.NewValue;
    }

    public double SmokeSpeed
    {
        get => smokeSpeed;
        set => SetValue(SmokeSpeedProperty, value);
    }

    public static readonly DependencyProperty SmokeSpeedProperty =
        DependencyProperty.Register(nameof(SmokeSpeed), typeof(double), typeof(FireRenderer), new PropertyMetadata(1.0d, OnSmokeSpeedChanged));

    private static void OnSmokeSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.smokeSpeed = (float)(double)e.NewValue;
    }

    public double Brightness
    {
        get => brightness;
        set => SetValue(BrightnessProperty, value);
    }

    public static readonly DependencyProperty BrightnessProperty =
        DependencyProperty.Register(nameof(Brightness), typeof(double), typeof(FireRenderer), new PropertyMetadata(1.0d, OnBrightnessChanged));

    private static void OnBrightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.brightness = (float)(double)e.NewValue;
    }

    public double ParticleSize
    {
        get => particleSize;
        set => SetValue(ParticleSizeProperty, value);
    }

    public static readonly DependencyProperty ParticleSizeProperty =
        DependencyProperty.Register(nameof(ParticleSize), typeof(double), typeof(FireRenderer), new PropertyMetadata(1.0d, OnParticleSizeChanged));

    private static void OnParticleSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.particleSize = (float)(double)e.NewValue;
    }

    public double ParticleDensity
    {
        get => particleDensity;
        set => SetValue(ParticleDensityProperty, value);
    }

    public static readonly DependencyProperty ParticleDensityProperty =
        DependencyProperty.Register(nameof(ParticleDensity), typeof(double), typeof(FireRenderer), new PropertyMetadata(15.0d, OnParticleDensityChanged));

    private static void OnParticleDensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.particleDensity = (float)(double)e.NewValue;
    }

    public double SmokeAmount
    {
        get => smokeAmount;
        set => SetValue(SmokeAmountProperty, value);
    }

    public static readonly DependencyProperty SmokeAmountProperty =
        DependencyProperty.Register(nameof(SmokeAmount), typeof(double), typeof(FireRenderer), new PropertyMetadata(1.0d, OnSmokeAmountChanged));

    private static void OnSmokeAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.smokeAmount = (float)(double)e.NewValue;
    }

    public double FlameHeight
    {
        get => flameHeight;
        set => SetValue(FlameHeightProperty, value);
    }

    public static readonly DependencyProperty FlameHeightProperty =
        DependencyProperty.Register(nameof(FlameHeight), typeof(double), typeof(FireRenderer), new PropertyMetadata(1.0d, OnFlameHeightChanged));

    private static void OnFlameHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.flameHeight = (float)(double)e.NewValue;
    }

    public double VignetteStrength
    {
        get => vignetteStrength;
        set => SetValue(VignetteStrengthProperty, value);
    }

    public static readonly DependencyProperty VignetteStrengthProperty =
        DependencyProperty.Register(nameof(VignetteStrength), typeof(double), typeof(FireRenderer), new PropertyMetadata(1.0d, OnVignetteStrengthChanged));

    private static void OnVignetteStrengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FireRenderer)d;
        ctl.vignetteStrength = (float)(double)e.NewValue;
    }
}
