namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public ParticleGlobeTheme Theme
    {
        get { return (ParticleGlobeTheme)GetValue(ThemeProperty); }
        set { SetValue(ThemeProperty, value); }
    }

    public static readonly DependencyProperty ThemeProperty =
        DependencyProperty.Register(nameof(Theme), typeof(ParticleGlobeTheme), typeof(ParticleGlobe), new PropertyMetadata(ParticleGlobeTheme.Default, OnSettingsPropertyChanged));

    private readonly record struct Palette(Vector4 Land, Vector4 Ocean, Vector4 Atmos, Vector4 Grid, Vector4 Arc, Vector4 Accent, Vector4 Star, Vector4 Border);

    private static Palette GetPalette(ParticleGlobeTheme theme) => theme switch
    {
        ParticleGlobeTheme.Cyan => new Palette(
            new(0.20f, 0.90f, 1.00f, 0.90f), new(0.02f, 0.30f, 0.55f, 0.55f), new(0.10f, 0.80f, 1.00f, 0.60f),
            new(0.20f, 0.80f, 1.00f, 0.22f), new(0.90f, 1.00f, 1.00f, 0.95f), new(0.70f, 1.00f, 1.00f, 1.00f),
            new(0.70f, 0.95f, 1.00f, 0.90f), new(1.00f, 1.00f, 1.00f, 1f)),
        ParticleGlobeTheme.Amber => new Palette(
            new(1.00f, 0.72f, 0.20f, 0.90f), new(0.55f, 0.25f, 0.03f, 0.55f), new(1.00f, 0.55f, 0.10f, 0.55f),
            new(1.00f, 0.60f, 0.15f, 0.22f), new(1.00f, 0.95f, 0.70f, 0.95f), new(1.00f, 0.85f, 0.40f, 1.00f),
            new(1.00f, 0.90f, 0.70f, 0.85f), new(1.00f, 1.00f, 0.85f, 1f)),
        ParticleGlobeTheme.Matrix => new Palette(
            new(0.15f, 1.00f, 0.30f, 0.90f), new(0.00f, 0.35f, 0.10f, 0.55f), new(0.10f, 0.90f, 0.30f, 0.50f),
            new(0.10f, 0.80f, 0.25f, 0.20f), new(0.75f, 1.00f, 0.75f, 0.95f), new(0.50f, 1.00f, 0.60f, 1.00f),
            new(0.40f, 1.00f, 0.50f, 0.80f), new(0.85f, 1.00f, 0.85f, 1f)),
        ParticleGlobeTheme.Hologram => new Palette(
            new(0.55f, 0.60f, 1.00f, 0.85f), new(0.45f, 0.15f, 0.75f, 0.55f), new(0.80f, 0.40f, 1.00f, 0.55f),
            new(0.70f, 0.50f, 1.00f, 0.22f), new(1.00f, 0.40f, 0.85f, 0.95f), new(0.40f, 1.00f, 0.95f, 1.00f),
            new(0.85f, 0.80f, 1.00f, 0.85f), new(0.40f, 1.00f, 0.95f, 1f)),
        _ => new Palette(DefLand, DefOcean, DefAtmos, DefGrid, DefArc, DefAccent, DefStar, new(1f, 0.82f, 0.35f, 1f)),
    };
}
