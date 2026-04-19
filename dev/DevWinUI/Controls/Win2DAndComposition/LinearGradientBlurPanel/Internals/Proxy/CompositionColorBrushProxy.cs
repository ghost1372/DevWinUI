namespace DevWinUI;

internal partial class CompositionColorBrushProxy : CompositionBrushProxy, ICompositionColorBrush
{
    public CompositionColorBrushProxy(CompositionColorBrush colorBrush)
        : base(colorBrush) { }

    public Color Color
    {
        get => ((CompositionColorBrush)RawObject).Color;
        set => ((CompositionColorBrush)RawObject).Color = value;
    }
}
