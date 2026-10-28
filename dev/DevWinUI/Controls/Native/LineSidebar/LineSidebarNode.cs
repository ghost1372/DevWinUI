using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

internal sealed partial class LineSidebarNode
{
    public bool IsTick;
    public int ItemIndex;
    public ContentControl Row = null!;
    public Rectangle? Marker;
    public ScaleTransform? Scale;
    public TranslateTransform? Shift;
    public SolidColorBrush? LabelBrush;
    public TextBlock? IndexText;
    public double Cur, Target;
}
