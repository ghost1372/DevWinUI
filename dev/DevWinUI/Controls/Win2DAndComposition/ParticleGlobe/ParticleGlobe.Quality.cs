namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const float QualityMinKeep = 0.25f;

    public ParticleGlobeQuality Quality
    {
        get { return (ParticleGlobeQuality)GetValue(QualityProperty); }
        set { SetValue(QualityProperty, value); }
    }

    public static readonly DependencyProperty QualityProperty =
        DependencyProperty.Register(nameof(Quality), typeof(ParticleGlobeQuality), typeof(ParticleGlobe), new PropertyMetadata(ParticleGlobeQuality.Auto, OnSettingsPropertyChanged));

    public double QualityTargetFps
    {
        get { return (double)GetValue(QualityTargetFpsProperty); }
        set { SetValue(QualityTargetFpsProperty, value); }
    }

    public static readonly DependencyProperty QualityTargetFpsProperty =
        DependencyProperty.Register(nameof(QualityTargetFps), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(55.0, OnSettingsPropertyChanged));

    public double EffectiveQuality => _qKeep;

    public double MeasuredFps => _qEmaMs > 0f ? 1000.0 / _qEmaMs : 0.0;

    private ParticleGlobeQuality _quality = ParticleGlobeQuality.Auto;
    private float _qTargetFps = 55f;
    private float _qKeep = 1f;
    private float _qEmaMs;
    private float _qCalm;

    private void SyncQualitySettings()
    {
        _showProfiler = ShowProfiler;
        SyncParallelSettings();
        _quality = Quality;
        _qTargetFps = (float)Math.Clamp(QualityTargetFps, 30.0, 144.0);
    }

    private void AdvanceQuality(float dt, bool busy)
    {
        float keep;
        switch (_quality)
        {
            case ParticleGlobeQuality.High: keep = 1f; break;
            case ParticleGlobeQuality.Medium: keep = 0.6f; break;
            case ParticleGlobeQuality.Low: keep = QualityMinKeep; break;
            default:
                keep = AutoKeep(dt, busy);
                break;
        }

        _qKeep += (keep - _qKeep) * (1f - MathF.Exp(-dt * 4f));
        if (MathF.Abs(_qKeep - keep) < 0.004f) _qKeep = keep;
    }

    private void QualitySample(float ms)
    {
        if (ms <= 0f || ms > 150f) return;
        _qEmaMs = _qEmaMs <= 0f ? ms : _qEmaMs + (ms - _qEmaMs) * 0.06f;
    }

    private float AutoKeep(float dt, bool busy)
    {
        if (_qEmaMs <= 0f) return _qKeepTarget;

        float budget = 1000f / _qTargetFps;
        float step = dt;
        if (_qEmaMs > budget * 1.1f)
        {
            if (_qCalm > 0f) _qCalmNeeded = MathF.Min(20f, _qCalmNeeded * 1.5f);
            _qCalm = 0f;
            _qKeepTarget = MathF.Max(QualityMinKeep, _qKeepTarget - 0.35f * step);
        }
        else if (_qKeepTarget < 1f && _qEmaMs <= budget * 1.03f)
        {
            if (!busy) _qCalm += step;
            if (_qCalm > _qCalmNeeded) _qKeepTarget = MathF.Min(1f, _qKeepTarget + 0.05f * step);
        }

        return _qKeepTarget;
    }

    private float _qKeepTarget = 1f;
    private float _qCalmNeeded = 2.5f;
}
