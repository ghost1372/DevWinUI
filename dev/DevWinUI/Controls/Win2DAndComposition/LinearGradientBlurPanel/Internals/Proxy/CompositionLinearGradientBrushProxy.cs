using CompositionNS = Microsoft.UI.Composition;

namespace DevWinUI;

internal partial class CompositionLinearGradientBrushProxy : CompositionBrushProxy, ICompositionLinearGradientBrush
{
    private CompositionColorGradientStopCollectionProxy? colorStops;

    public CompositionLinearGradientBrushProxy(CompositionLinearGradientBrush brush)
        : base(brush) { }
    public Vector2 StartPoint
    {
        get => ((CompositionLinearGradientBrush)RawObject).StartPoint;
        set => ((CompositionLinearGradientBrush)RawObject).StartPoint = value;
    }

    public Vector2 EndPoint
    {
        get => ((CompositionLinearGradientBrush)RawObject).EndPoint;
        set => ((CompositionLinearGradientBrush)RawObject).EndPoint = value;
    }

    public CompositionMappingModeEx MappingMode
    {
        get => MapCompositionMappingMode(((CompositionLinearGradientBrush)RawObject).MappingMode);
        set => ((CompositionLinearGradientBrush)RawObject).MappingMode = MapCompositionMappingMode(value);
    }

    public ICompositionColorGradientStopCollection ColorStops => IsDisposed ? null! : (colorStops ??= new CompositionColorGradientStopCollectionProxy(((CompositionNS.CompositionLinearGradientBrush)RawObject).ColorStops));

    protected override void DisposeCore()
    {
        base.DisposeCore();

        colorStops = null!;
    }

    private static CompositionNS.CompositionMappingMode MapCompositionMappingMode(CompositionMappingModeEx mappingMode)
    {
        switch (mappingMode)
        {
            case CompositionMappingModeEx.Relative:
                return CompositionNS.CompositionMappingMode.Relative;

            default:
            case CompositionMappingModeEx.Absolute:
                return CompositionNS.CompositionMappingMode.Absolute;
        }
    }

    private static CompositionMappingModeEx MapCompositionMappingMode(CompositionNS.CompositionMappingMode mappingMode)
    {
        switch (mappingMode)
        {
            case CompositionNS.CompositionMappingMode.Relative:
                return CompositionMappingModeEx.Relative;

            default:
            case CompositionNS.CompositionMappingMode.Absolute:
                return CompositionMappingModeEx.Absolute;
        }
    }
}
