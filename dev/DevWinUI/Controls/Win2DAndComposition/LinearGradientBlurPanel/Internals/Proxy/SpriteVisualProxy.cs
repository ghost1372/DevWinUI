namespace DevWinUI;

internal partial class SpriteVisualProxy : CompositionObjectProxy, ISpriteVisual
{
    private VisualCollectionProxy children;
    private ICompositionBrush? compositionBrush;

    public SpriteVisualProxy(SpriteVisual visual) : base(visual)
    {
        this.children = new VisualCollectionProxy(((SpriteVisual)RawObject).Children);
    }

    public Vector2 RelativeSizeAdjustment
    {
        get => ((SpriteVisual)RawObject).RelativeSizeAdjustment;
        set => ((SpriteVisual)RawObject).RelativeSizeAdjustment = value;
    }

    public IVisualCollection Children => IsDisposed ? null! : (children ??= new VisualCollectionProxy(((SpriteVisual)RawObject).Children));

    public ICompositionBrush? Brush
    {
        get => GetCompositionBrush();
        set => ((SpriteVisual)RawObject).Brush = (CompositionBrush?)value?.RawObject;
    }

    private ICompositionBrush? GetCompositionBrush()
    {
        var brush = ((SpriteVisual)RawObject).Brush;

        if (brush == null)
        {
            compositionBrush = null;
        }
        else
        {
            if (!ReferenceEquals(compositionBrush?.RawObject, brush))
            {
                if (brush is CompositionEffectBrush effectBrush)
                {
                    compositionBrush = new CompositionEffectBrushProxy(effectBrush);
                }
                else if (brush is CompositionLinearGradientBrush linearGradientBrush)
                {
                    compositionBrush = new CompositionLinearGradientBrushProxy(linearGradientBrush);
                }
                else if (brush is CompositionColorBrush colorBrush)
                {
                    compositionBrush = new CompositionColorBrushProxy(colorBrush);
                }
                else
                {
                    compositionBrush = new CompositionBrushProxy(brush);
                }
            }
        }
        return compositionBrush;
    }

    protected override void DisposeCore()
    {
        base.DisposeCore();

        children = null!;
        compositionBrush = null!;
    }
}
