using Windows.Graphics.Effects;

namespace DevWinUI;

internal partial class CompositorProxy : ICompositor
{
    private readonly Compositor compositor;

    public CompositorProxy(Compositor compositor)
    {
        this.compositor = compositor;
    }

    public object RawObject => compositor;

    public ICompositionBrush CreateBackdropBrush()
    {
        return new CompositionBrushProxy(compositor.CreateBackdropBrush());
    }

    public ICompositionColorBrush CreateColorBrush()
    {
        return new CompositionColorBrushProxy(compositor.CreateColorBrush());
    }

    public ICompositionColorBrush CreateColorBrush(Color color)
    {
        return new CompositionColorBrushProxy(compositor.CreateColorBrush(color));
    }

    public ICompositionColorGradientStop CreateColorGradientStop()
    {
        return new CompositionColorGradientStopProxy(compositor.CreateColorGradientStop());
    }

    public ICompositionColorGradientStop CreateColorGradientStop(float offset, Color color)
    {
        return new CompositionColorGradientStopProxy(compositor.CreateColorGradientStop(offset, color));
    }

    public ICompositionEffectBrush CreateEffectBrush(IGraphicsEffect effect, string[] animatableProperties)
    {
        return new CompositionEffectBrushProxy(compositor.CreateEffectFactory(effect, animatableProperties).CreateBrush());
    }

    public ICompositionLinearGradientBrush CreateLinearGradientBrush()
    {
        return new CompositionLinearGradientBrushProxy(compositor.CreateLinearGradientBrush());
    }

    public ISpriteVisual CreateSpriteVisual()
    {
        return new SpriteVisualProxy(compositor.CreateSpriteVisual());
    }

    public IExpressionAnimation CreateExpressionAnimation()
    {
        return new ExpressionAnimationProxy(compositor.CreateExpressionAnimation());
    }

    public IExpressionAnimation CreateExpressionAnimation(string expression)
    {
        return new ExpressionAnimationProxy(compositor.CreateExpressionAnimation(expression));
    }

    public ICompositionPropertySet CreatePropertySet()
    {
        return new CompositionPropertySetProxy(compositor.CreatePropertySet());
    }
}
