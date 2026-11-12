namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const int ParallelMinParticles = 4096;

    public bool UseParallelParticles
    {
        get { return (bool)GetValue(UseParallelParticlesProperty); }
        set { SetValue(UseParallelParticlesProperty, value); }
    }

    public static readonly DependencyProperty UseParallelParticlesProperty =
        DependencyProperty.Register(nameof(UseParallelParticles), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    private bool _parallel;
    private Vector2[] _bufPos = Array.Empty<Vector2>();
    private Vector4[] _bufTint = Array.Empty<Vector4>();
    private float[] _bufScale = Array.Empty<float>();

    private void SyncParallelSettings()
    {
        _parallel = UseParallelParticles;
        if (!_parallel && _bufScale.Length != 0)
        {
            _bufPos = Array.Empty<Vector2>();
            _bufTint = Array.Empty<Vector4>();
            _bufScale = Array.Empty<float>();
        }
    }

    private void EnsureParticleBuffers(int n, out Vector2[] pos, out Vector4[] tint, out float[] scale)
    {
        if (_bufScale.Length < n)
        {
            _bufPos = new Vector2[n];
            _bufTint = new Vector4[n];
            _bufScale = new float[n];
        }

        pos = _bufPos; tint = _bufTint; scale = _bufScale;
    }
}
