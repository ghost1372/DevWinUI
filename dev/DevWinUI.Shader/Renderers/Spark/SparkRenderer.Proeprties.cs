using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class SparkRenderer
{
    private float speed = 1f;
    private float driftSpeed = 1f;

    private float particleScale = 1f;
    private float particleDensity = 10f;

    private float smokeSpeed = 1f;
    private float smokeIntensity = 1f;

    private float trailStrength = 1f;
    private float vignetteStrength = 1f;

    public double Speed
    {
        get => speed;
        set => SetValue(SpeedProperty, value);
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(SparkRenderer), new PropertyMetadata(1.0d, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.speed = (float)(double)e.NewValue;
    }

    public double DriftSpeed
    {
        get => driftSpeed;
        set => SetValue(DriftSpeedProperty, value);
    }

    public static readonly DependencyProperty DriftSpeedProperty =
        DependencyProperty.Register(nameof(DriftSpeed), typeof(double), typeof(SparkRenderer), new PropertyMetadata(1.0d, OnDriftSpeedChanged));

    private static void OnDriftSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.driftSpeed = (float)(double)e.NewValue;
    }

    public double ParticleScale
    {
        get => particleScale;
        set => SetValue(ParticleScaleProperty, value);
    }

    public static readonly DependencyProperty ParticleScaleProperty =
        DependencyProperty.Register(nameof(ParticleScale), typeof(double), typeof(SparkRenderer), new PropertyMetadata(1.0d, OnParticleScaleChanged));

    private static void OnParticleScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.particleScale = (float)(double)e.NewValue;
    }

    public double ParticleDensity
    {
        get => particleDensity;
        set => SetValue(ParticleDensityProperty, value);
    }

    public static readonly DependencyProperty ParticleDensityProperty =
        DependencyProperty.Register(nameof(ParticleDensity), typeof(double), typeof(SparkRenderer), new PropertyMetadata(10.0d, OnParticleDensityChanged));

    private static void OnParticleDensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.particleDensity = (float)(double)e.NewValue;
    }

    public double SmokeSpeed
    {
        get => smokeSpeed;
        set => SetValue(SmokeSpeedProperty, value);
    }

    public static readonly DependencyProperty SmokeSpeedProperty =
        DependencyProperty.Register(nameof(SmokeSpeed), typeof(double), typeof(SparkRenderer), new PropertyMetadata(1.0d, OnSmokeSpeedChanged));

    private static void OnSmokeSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.smokeSpeed = (float)(double)e.NewValue;
    }

    public double SmokeIntensity
    {
        get => smokeIntensity;
        set => SetValue(SmokeIntensityProperty, value);
    }

    public static readonly DependencyProperty SmokeIntensityProperty =
        DependencyProperty.Register(nameof(SmokeIntensity), typeof(double), typeof(SparkRenderer), new PropertyMetadata(1.0d, OnSmokeIntensityChanged));

    private static void OnSmokeIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.smokeIntensity = (float)(double)e.NewValue;
    }

    public double TrailStrength
    {
        get => trailStrength;
        set => SetValue(TrailStrengthProperty, value);
    }

    public static readonly DependencyProperty TrailStrengthProperty =
        DependencyProperty.Register(nameof(TrailStrength), typeof(double), typeof(SparkRenderer), new PropertyMetadata(1.0d, OnTrailStrengthChanged));

    private static void OnTrailStrengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.trailStrength = (float)(double)e.NewValue;
    }

    public double VignetteStrength
    {
        get => vignetteStrength;
        set => SetValue(VignetteStrengthProperty, value);
    }

    public static readonly DependencyProperty VignetteStrengthProperty =
        DependencyProperty.Register(nameof(VignetteStrength), typeof(double), typeof(SparkRenderer), new PropertyMetadata(1.0d, OnVignetteStrengthChanged));

    private static void OnVignetteStrengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SparkRenderer)d;
        ctl.vignetteStrength = (float)(double)e.NewValue;
    }
}
