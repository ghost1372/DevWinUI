namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public ParticleGlobeSelectionEffects SelectionEffects
    {
        get { return (ParticleGlobeSelectionEffects)GetValue(SelectionEffectsProperty); }
        set { SetValue(SelectionEffectsProperty, value); }
    }

    public static readonly DependencyProperty SelectionEffectsProperty =
        DependencyProperty.Register(nameof(SelectionEffects), typeof(ParticleGlobeSelectionEffects), typeof(ParticleGlobe),
            new PropertyMetadata(ParticleGlobeSelectionEffects.Highlight | ParticleGlobeSelectionEffects.Pulse | ParticleGlobeSelectionEffects.Outline, OnSettingsPropertyChanged));

    public double SelectionDimAmount
    {
        get { return (double)GetValue(SelectionDimAmountProperty); }
        set { SetValue(SelectionDimAmountProperty, value); }
    }

    public static readonly DependencyProperty SelectionDimAmountProperty =
        DependencyProperty.Register(nameof(SelectionDimAmount), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.7, OnSettingsPropertyChanged));

    private ParticleGlobeSelectionEffects _selFx = ParticleGlobeSelectionEffects.Highlight | ParticleGlobeSelectionEffects.Pulse | ParticleGlobeSelectionEffects.Outline;
    private float _selDim = 0.7f;

    private void SyncSelectionSettings()
    {
        _selFx = SelectionEffects;
        _selDim = (float)Math.Clamp(SelectionDimAmount, 0.0, 1.0);
    }
}
