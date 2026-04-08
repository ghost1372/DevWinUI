namespace DevWinUI;

internal partial class CompositionColorGradientStopProxy : CompositionObjectProxy, ICompositionColorGradientStop
{
    public CompositionColorGradientStopProxy(CompositionColorGradientStop stop)
        : base(stop) { }

    public float Offset
    {
        get => ((CompositionColorGradientStop)RawObject).Offset;
        set => ((CompositionColorGradientStop)RawObject).Offset = value;
    }

    public Color Color
    {
        get => ((CompositionColorGradientStop)RawObject).Color;
        set => ((CompositionColorGradientStop)RawObject).Color = value;
    }
}
