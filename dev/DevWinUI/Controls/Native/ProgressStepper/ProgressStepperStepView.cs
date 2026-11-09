using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

internal sealed partial class ProgressStepperStepView
{
    public ProgressStep Step = null!;
    public int Index;
    public Grid Row = null!;
    public ProgressStepperMarkerView Marker = new();
    public Rectangle? Rail;
    public FrameworkElement Title = null!;
    public FrameworkElement? Description;
    public Grid ChildrenWrap = null!;
    public StackPanel ChildrenPanel = null!;
    public List<ProgressStepperChildView> Children = new();
    public bool? Expanded;
    public Storyboard? HeightSb;
}
