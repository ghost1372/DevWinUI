using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

internal sealed partial class MagicKeyboardKeyVisual
{
    public KeyDef Def = null!;
    public Grid Root = null!;
    public Path Face = null!;
    public TranslateTransform Dip = null!;
    public Ellipse? Led;
    public TextBlock? Letter;
    public StackPanel? Dual;
    public Color FaceTop, FaceBottom;
}
