using Windows.Graphics.Effects;

namespace DevWinUI;

internal partial class LinearGradientBlurHelper : LinearGradientBlurHelperBase<Visual, CompositionPropertySet>
{
    public LinearGradientBlurHelper(Compositor compositor) : base(new CompositorProxy(compositor))
    {
    }

    protected override IGraphicsEffectSource CreateEffectSourceParameter(string name)
    {
        return new CompositionEffectSourceParameter(name);
    }
}
