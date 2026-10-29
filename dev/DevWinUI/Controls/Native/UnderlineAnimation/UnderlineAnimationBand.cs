using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

internal sealed partial class UnderlineAnimationBand
{
    public UnderlineAnimationBand(double width)
    {
        Width = width;
    }

    public double Width { get; }
    public Rectangle? Rect { get; set; }
    public ScaleTransform? Scale { get; set; }
    public Path? Wave { get; set; }
    public double Dash { get; set; }
}
