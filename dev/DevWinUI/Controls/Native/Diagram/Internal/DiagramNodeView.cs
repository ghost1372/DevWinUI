namespace DevWinUI;

internal sealed partial class DiagramNodeView
{
    public DiagramNode Node = null!;
    public SolidColorBrush HueBrush = null!;
    public double Width;
    public double FixedHeight;
    public double LastHeight;
    public int Z;

    public ContentControl? Card;
    public Border? Face;
    public Button? Toggler;
    public List<TextBlock>? Muted;

    public double BaseLeft, BaseTop;
    public bool Hover, Focus, Drag, ComboOpen, Selected, Match = true;

    public readonly List<DiagramNodeView> Children = new();
    public int ParentCount;

    public readonly List<DiagramNodeView> LIn = new();
    public readonly List<DiagramNodeView> LOut = new();
    public int Layer, Pending, Order;
    public double Center, RawLeft, RawTop;

    public readonly Dictionary<int, DiagramTypeRef> Bindings = new();
    public Dictionary<DiagramPin, DiagramPinVisual>? Pins;

    public bool Hot => Hover || Focus || Drag;
    public bool Pinned => Focus || Drag || ComboOpen;
}
