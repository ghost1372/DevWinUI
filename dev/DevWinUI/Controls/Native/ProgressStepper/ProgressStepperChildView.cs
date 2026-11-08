namespace DevWinUI;

internal sealed partial class ProgressStepperChildView
{
    public ProgressStep Step = null!;
    public int Index;
    public ProgressStepperMarkerView Marker = new();
    public TextBlock Text = null!;
}
