// https://github.com/jayfunc/BetterLyrics

namespace DevWinUI;

internal partial class RenderLyricsChar : BaseRenderLyrics
{
    public Rect LayoutRect { get; private set; }

    public ValueTransition<double> ScaleTransition { get; set; }
    public ValueTransition<double> GlowTransition { get; set; }
    public ValueTransition<double> FloatTransition { get; set; }

    public CropEffect Crop { get; }
    public GaussianBlurEffect Glow { get; }

    public double ProgressPlayed { get; set; } = 0; // 0~1

    public RenderLyricsChar(BaseLyric lyricsChars, Rect layoutRect) : base(lyricsChars)
    {
        ScaleTransition = new(
            initialValue: 1.0,
            AnimationEasingHelper.GetInterpolatorByEasingType<double>(AnimationEasingType.Sine),
            defaultTotalDuration: BetterLyricTimeSpanHelper.AnimationDuration.TotalSeconds
        );
        GlowTransition = new(
            initialValue: 0,
            AnimationEasingHelper.GetInterpolatorByEasingType<double>(AnimationEasingType.Sine),
            defaultTotalDuration: BetterLyricTimeSpanHelper.AnimationDuration.TotalSeconds
        );
        FloatTransition = new(
            initialValue: 0,
            AnimationEasingHelper.GetInterpolatorByEasingType<double>(AnimationEasingType.Sine),
            defaultTotalDuration: BetterLyricTimeSpanHelper.LongAnimationDuration.TotalSeconds
        );
        LayoutRect = layoutRect;
        Crop = new CropEffect { BorderMode = EffectBorderMode.Hard };
        Glow = new GaussianBlurEffect { Source = Crop, BorderMode = EffectBorderMode.Soft };
    }

    public void Update(TimeSpan elapsedTime)
    {
        ScaleTransition.Update(elapsedTime);
        GlowTransition.Update(elapsedTime);
        FloatTransition.Update(elapsedTime);
    }

    public void DisposeEffects()
    {
        Crop?.Dispose();
        Glow?.Dispose();
    }

}
