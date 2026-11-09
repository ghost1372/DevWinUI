namespace DevWinUI;

internal sealed partial class DonutSlice
{
    public required Microsoft.UI.Xaml.Shapes.Path Path { get; init; }
    public required Button LegendItem { get; init; }
    public double Value { get; init; }
    public double StartAngle { get; init; }
    public double EndAngle { get; init; }
    public Vector3 Lift { get; init; }
}
