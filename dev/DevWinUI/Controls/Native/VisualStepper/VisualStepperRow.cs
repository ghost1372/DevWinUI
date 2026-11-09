using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

internal sealed partial class VisualStepperRow
{
    public VisualStepperStep Step = null!;
    public Grid Root = null!;
    public Rectangle Track = null!;
    public Rectangle Progress = null!;
    public ScaleTransform Fill = null!;
    public TextBlock Title = null!;
    public TextBlock Description = null!;
}
