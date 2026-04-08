namespace DevWinUI;

internal partial class CompositionEffectBrushProxy : CompositionBrushProxy, ICompositionEffectBrush
{
    public CompositionEffectBrushProxy(CompositionEffectBrush effectBrush)
        : base(effectBrush) { }

    public void SetSourceParameter(string name, ICompositionBrush source)
    {
        ((CompositionEffectBrush)RawObject).SetSourceParameter(name, (CompositionBrush)source.RawObject);
    }
}
