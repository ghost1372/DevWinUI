using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

internal sealed partial class DiagramLinkDrag
{
    public DiagramNodeView View = null!;
    public DiagramPin Pin = null!;
    public Path Line = null!;
    public Border Handle = null!;
}
