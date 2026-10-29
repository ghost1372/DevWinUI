using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

internal sealed partial class TextHighlightBand
{
    public TextHighlightBand(Rectangle rect, ScaleTransform scale, double width)
    {
        Rect = rect;
        Scale = scale;
        Width = width;
    }

    public Rectangle Rect { get; }
    public ScaleTransform Scale { get; }
    public double Width { get; }
}
