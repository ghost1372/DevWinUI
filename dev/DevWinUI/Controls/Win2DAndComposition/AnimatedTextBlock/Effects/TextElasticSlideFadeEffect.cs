namespace DevWinUI;

public partial class TextElasticSlideFadeEffect : ITextEffect
{
    public TextSlideFadeEffectDirection Direction { get; set; } = TextSlideFadeEffectDirection.BottomToTop;

    public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromMilliseconds(450);

    public TimeSpan DelayPerCluster { get; set; } = TimeSpan.Zero;

    public float SlideDistanceMultiplier { get; set; } = 1f;
    public void Update(
        string oldText,
        string newText,
        List<TextDiffResult> diffResults,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        AnimatedTextBlockRedrawState state,
        ICanvasAnimatedControl canvas,
        CanvasAnimatedUpdateEventArgs args)
    {
    }

    public void DrawText(
        string oldText,
        string newText,
        List<TextDiffResult> diffResults,
        CanvasTextLayout oldTextLayout,
        CanvasTextLayout newTextLayout,
        CanvasTextFormat textFormat,
        Color textColor,
        CanvasLinearGradientBrush gradientBrush,
        AnimatedTextBlockRedrawState state,
        CanvasDrawingSession drawingSession,
        CanvasAnimatedDrawEventArgs args)
    {
        var ds = drawingSession;

        if (state == AnimatedTextBlockRedrawState.Idle)
        {
            ds.DrawTextLayout(newTextLayout, 0, 0, textColor);
            return;
        }

        float progress = 0f;

        if (diffResults?.Count > 0)
        {
            progress = diffResults
                .Where(x => x.NewGlyphCluster != null)
                .Select(x => x.NewGlyphCluster.Progress)
                .DefaultIfEmpty(0f)
                .Max();
        }

        progress = (float)AnimationEasingHelper.Ease<float>(progress, AnimationEaseMode.Out, AnimationEasingHelper.EaseInCubic);

        float width = (float)Math.Max(oldTextLayout.LayoutBounds.Width, newTextLayout.LayoutBounds.Width);
        float height = (float)Math.Max(oldTextLayout.LayoutBounds.Height, newTextLayout.LayoutBounds.Height);

        width *= SlideDistanceMultiplier;
        height *= SlideDistanceMultiplier;

        Vector2 oldOffset;
        Vector2 newOffset;

        switch (Direction)
        {
            case TextSlideFadeEffectDirection.BottomToTop:
                oldOffset = new Vector2(0, -height * progress);
                newOffset = new Vector2(0, height * (1f - progress));
                break;

            case TextSlideFadeEffectDirection.TopToBottom:
                oldOffset = new Vector2(0, height * progress);
                newOffset = new Vector2(0, -height * (1f - progress));
                break;

            case TextSlideFadeEffectDirection.RightToLeft:
                oldOffset = new Vector2(-width * progress, 0);
                newOffset = new Vector2(width * (1f - progress), 0);
                break;

            case TextSlideFadeEffectDirection.LeftToRight:
                oldOffset = new Vector2(width * progress, 0);
                newOffset = new Vector2(-width * (1f - progress), 0);
                break;

            default:
                oldOffset = Vector2.Zero;
                newOffset = Vector2.Zero;
                break;
        }

        // Old text
        using (ds.CreateLayer(1f - progress))
        {
            ds.Transform = Matrix3x2.CreateTranslation(oldOffset);
            ds.DrawTextLayout(oldTextLayout, 0, 0, textColor);
            ds.Transform = Matrix3x2.Identity;
        }

        // New text
        using (ds.CreateLayer(progress))
        {
            ds.Transform = Matrix3x2.CreateTranslation(newOffset);
            ds.DrawTextLayout(newTextLayout, 0, 0, textColor);
            ds.Transform = Matrix3x2.Identity;
        }
    }
}
