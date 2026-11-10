namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public bool IsIntroEnabled
    {
        get { return (bool)GetValue(IsIntroEnabledProperty); }
        set { SetValue(IsIntroEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsIntroEnabledProperty =
        DependencyProperty.Register(nameof(IsIntroEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public double IntroDuration
    {
        get { return (double)GetValue(IntroDurationProperty); }
        set { SetValue(IntroDurationProperty, value); }
    }

    public static readonly DependencyProperty IntroDurationProperty =
        DependencyProperty.Register(nameof(IntroDuration), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(3.0, OnSettingsPropertyChanged));

    public void ReplayIntro()
    {
        _introReplay = true;
    }

    private bool _introEnabled = true, _introPlayed, _introReplay;
    private float _introDuration = 3f;
    private float _introT = 1f;

    private void SyncIntroSettings()
    {
        _introEnabled = IsIntroEnabled;
        _introDuration = (float)Math.Clamp(IntroDuration, 0.3, 15.0);
    }

    // Called after a particle set is built: the first one assembles from scratch.
    private void OnParticlesRebuilt()
    {
        if (_introPlayed || !_introEnabled) { _introPlayed = true; return; }
        _introPlayed = true;
        _introT = 0f;
    }

    private void AdvanceIntro(float dt)
    {
        if (_introReplay)
        {
            _introReplay = false;
            _introT = 0f;
        }

        if (_introT < 1f) _introT = MathF.Min(1f, _introT + dt / _introDuration);
    }

    private static float Hash01(float s, float k)
    {
        float v = MathF.Sin(s * k) * 43758.5453f;
        return v - MathF.Floor(v);
    }

    // Scatter start for a particle: a random point on a wide shell around the globe.
    private static Vector3 IntroOrigin(float seed)
    {
        float th = Hash01(seed, 127.1f) * MathF.Tau;
        float cz = Hash01(seed, 311.7f) * 2f - 1f;
        float r = MathF.Sqrt(MathF.Max(0f, 1f - cz * cz));
        float dist = 1.8f + 2.2f * Hash01(seed, 74.7f);
        return new Vector3(r * MathF.Cos(th), cz, r * MathF.Sin(th)) * dist;
    }
}
