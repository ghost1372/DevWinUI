using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

internal sealed partial class BranchedMenuEntry
{
    public string Value = "";
    public object Item = null!;
    public BranchedMenuSection Section = null!;
    public Button Button = null!;
    public TextBlock Label = null!;
    public FontIcon? Icon;
    public Path? Accent;
    public double Dash;
    public Storyboard? DrawStoryboard;
    public bool IsHeader;
}
