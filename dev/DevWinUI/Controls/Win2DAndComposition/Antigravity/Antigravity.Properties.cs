namespace DevWinUI;

public sealed partial class Antigravity
{
    public int ParticleCount
    {
        get { return (int)GetValue(ParticleCountProperty); }
        set { SetValue(ParticleCountProperty, value); }
    }

    public static readonly DependencyProperty ParticleCountProperty =
        DependencyProperty.Register(nameof(ParticleCount), typeof(int), typeof(Antigravity), new PropertyMetadata(1200, OnSettingsChanged));

    public double MagnetRadius
    {
        get { return (double)GetValue(MagnetRadiusProperty); }
        set { SetValue(MagnetRadiusProperty, value); }
    }

    public static readonly DependencyProperty MagnetRadiusProperty =
        DependencyProperty.Register(nameof(MagnetRadius), typeof(double), typeof(Antigravity), new PropertyMetadata(170d, OnSettingsChanged));

    public double RingRadius
    {
        get { return (double)GetValue(RingRadiusProperty); }
        set { SetValue(RingRadiusProperty, value); }
    }

    public static readonly DependencyProperty RingRadiusProperty =
        DependencyProperty.Register(nameof(RingRadius), typeof(double), typeof(Antigravity), new PropertyMetadata(120d, OnSettingsChanged));

    public double WaveSpeed
    {
        get { return (double)GetValue(WaveSpeedProperty); }
        set { SetValue(WaveSpeedProperty, value); }
    }

    public static readonly DependencyProperty WaveSpeedProperty =
        DependencyProperty.Register(nameof(WaveSpeed), typeof(double), typeof(Antigravity), new PropertyMetadata(0.4d, OnSettingsChanged));

    public double WaveAmplitude
    {
        get { return (double)GetValue(WaveAmplitudeProperty); }
        set { SetValue(WaveAmplitudeProperty, value); }
    }

    public static readonly DependencyProperty WaveAmplitudeProperty =
        DependencyProperty.Register(nameof(WaveAmplitude), typeof(double), typeof(Antigravity), new PropertyMetadata(1d, OnSettingsChanged));

    public int WaveCount
    {
        get { return (int)GetValue(WaveCountProperty); }
        set { SetValue(WaveCountProperty, value); }
    }

    public static readonly DependencyProperty WaveCountProperty =
        DependencyProperty.Register(nameof(WaveCount), typeof(int), typeof(Antigravity), new PropertyMetadata(3, OnSettingsChanged));

    public double ParticleSize
    {
        get { return (double)GetValue(ParticleSizeProperty); }
        set { SetValue(ParticleSizeProperty, value); }
    }

    public static readonly DependencyProperty ParticleSizeProperty =
        DependencyProperty.Register(nameof(ParticleSize), typeof(double), typeof(Antigravity), new PropertyMetadata(0.9d, OnSettingsChanged));

    public double LerpSpeed
    {
        get { return (double)GetValue(LerpSpeedProperty); }
        set { SetValue(LerpSpeedProperty, value); }
    }

    public static readonly DependencyProperty LerpSpeedProperty =
        DependencyProperty.Register(nameof(LerpSpeed), typeof(double), typeof(Antigravity), new PropertyMetadata(0.05d, OnSettingsChanged));

    public Brush Palette
    {
        get { return (Brush)GetValue(PaletteProperty); }
        set { SetValue(PaletteProperty, value); }
    }

    public static readonly DependencyProperty PaletteProperty =
        DependencyProperty.Register(nameof(Palette), typeof(Brush), typeof(Antigravity), new PropertyMetadata(null, OnPaletteChanged));

    public AntigravityColorMode ColorMode
    {
        get { return (AntigravityColorMode)GetValue(ColorModeProperty); }
        set { SetValue(ColorModeProperty, value); }
    }

    public static readonly DependencyProperty ColorModeProperty =
        DependencyProperty.Register(nameof(ColorMode), typeof(AntigravityColorMode), typeof(Antigravity), new PropertyMetadata(AntigravityColorMode.Conic, OnSettingsChanged));

    public double ColorSpeed
    {
        get { return (double)GetValue(ColorSpeedProperty); }
        set { SetValue(ColorSpeedProperty, value); }
    }

    public static readonly DependencyProperty ColorSpeedProperty =
        DependencyProperty.Register(nameof(ColorSpeed), typeof(double), typeof(Antigravity), new PropertyMetadata(0.05d, OnSettingsChanged));

    public bool AutoAnimate
    {
        get { return (bool)GetValue(AutoAnimateProperty); }
        set { SetValue(AutoAnimateProperty, value); }
    }

    public static readonly DependencyProperty AutoAnimateProperty =
        DependencyProperty.Register(nameof(AutoAnimate), typeof(bool), typeof(Antigravity), new PropertyMetadata(true, OnSettingsChanged));

    public double ParticleVariance
    {
        get { return (double)GetValue(ParticleVarianceProperty); }
        set { SetValue(ParticleVarianceProperty, value); }
    }

    public static readonly DependencyProperty ParticleVarianceProperty =
        DependencyProperty.Register(nameof(ParticleVariance), typeof(double), typeof(Antigravity), new PropertyMetadata(1d, OnSettingsChanged));

    public double RotationSpeed
    {
        get { return (double)GetValue(RotationSpeedProperty); }
        set { SetValue(RotationSpeedProperty, value); }
    }

    public static readonly DependencyProperty RotationSpeedProperty =
        DependencyProperty.Register(nameof(RotationSpeed), typeof(double), typeof(Antigravity), new PropertyMetadata(0d, OnSettingsChanged));

    public double DepthFactor
    {
        get { return (double)GetValue(DepthFactorProperty); }
        set { SetValue(DepthFactorProperty, value); }
    }

    public static readonly DependencyProperty DepthFactorProperty =
        DependencyProperty.Register(nameof(DepthFactor), typeof(double), typeof(Antigravity), new PropertyMetadata(1d, OnSettingsChanged));

    public double PulseSpeed
    {
        get { return (double)GetValue(PulseSpeedProperty); }
        set { SetValue(PulseSpeedProperty, value); }
    }

    public static readonly DependencyProperty PulseSpeedProperty =
        DependencyProperty.Register(nameof(PulseSpeed), typeof(double), typeof(Antigravity), new PropertyMetadata(3d, OnSettingsChanged));

    public AntigravityParticleShape ParticleShape
    {
        get { return (AntigravityParticleShape)GetValue(ParticleShapeProperty); }
        set { SetValue(ParticleShapeProperty, value); }
    }

    public static readonly DependencyProperty ParticleShapeProperty =
        DependencyProperty.Register(nameof(ParticleShape), typeof(AntigravityParticleShape), typeof(Antigravity), new PropertyMetadata(AntigravityParticleShape.Capsule, OnSettingsChanged));

    public double FieldStrength
    {
        get { return (double)GetValue(FieldStrengthProperty); }
        set { SetValue(FieldStrengthProperty, value); }
    }

    public static readonly DependencyProperty FieldStrengthProperty =
        DependencyProperty.Register(nameof(FieldStrength), typeof(double), typeof(Antigravity), new PropertyMetadata(10d, OnSettingsChanged));

    public double Ambient
    {
        get { return (double)GetValue(AmbientProperty); }
        set { SetValue(AmbientProperty, value); }
    }

    public static readonly DependencyProperty AmbientProperty =
        DependencyProperty.Register(nameof(Ambient), typeof(double), typeof(Antigravity), new PropertyMetadata(0.25d, OnSettingsChanged));

    public bool IsInteractive
    {
        get { return (bool)GetValue(IsInteractiveProperty); }
        set { SetValue(IsInteractiveProperty, value); }
    }

    public static readonly DependencyProperty IsInteractiveProperty =
        DependencyProperty.Register(nameof(IsInteractive), typeof(bool), typeof(Antigravity), new PropertyMetadata(true, OnSettingsChanged));

    public bool ClickBurst
    {
        get { return (bool)GetValue(ClickBurstProperty); }
        set { SetValue(ClickBurstProperty, value); }
    }

    public static readonly DependencyProperty ClickBurstProperty =
        DependencyProperty.Register(nameof(ClickBurst), typeof(bool), typeof(Antigravity), new PropertyMetadata(true, OnSettingsChanged));

    public bool IsPaused
    {
        get { return (bool)GetValue(IsPausedProperty); }
        set { SetValue(IsPausedProperty, value); }
    }

    public static readonly DependencyProperty IsPausedProperty =
        DependencyProperty.Register(nameof(IsPaused), typeof(bool), typeof(Antigravity), new PropertyMetadata(false, OnPausedChanged));

    public bool Intro
    {
        get { return (bool)GetValue(IntroProperty); }
        set { SetValue(IntroProperty, value); }
    }

    public static readonly DependencyProperty IntroProperty =
        DependencyProperty.Register(nameof(Intro), typeof(bool), typeof(Antigravity), new PropertyMetadata(true, OnSettingsChanged));
}
