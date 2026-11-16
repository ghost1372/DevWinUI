using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

internal sealed partial class DiagramEdgeView
{
    public DiagramEdge? Edge;
    public DiagramNodeView From = null!, To = null!;
    public string? Label;
    public bool Dashed;
    public Path? Path;
    public Border? LabelHost;
    public Size LabelSize;
    public Path? Hit;
    public DiagramPin? FromPin, ToPin;
    public string? Conflict, TypeName;
    public bool Animated;
    public int DashKind;
}
