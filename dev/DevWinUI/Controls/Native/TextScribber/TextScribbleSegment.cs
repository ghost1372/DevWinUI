using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

internal sealed partial class TextScribbleSegment
{
    public TextScribbleSegment(Path path, double dash, double length)
    {
        Path = path;
        Dash = dash;
        Length = length;
    }

    public Path Path { get; }
    public double Dash { get; }
    public double Length { get; }
}
