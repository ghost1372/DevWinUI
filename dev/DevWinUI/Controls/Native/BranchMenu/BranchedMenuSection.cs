namespace DevWinUI;

internal sealed partial class BranchedMenuSection
{
    public int Index;
    public bool HasChildren;
    public bool IsOpen;
    public double ContentHeight;
    public Button Header = null!;
    public TextBlock Label = null!;
    public FontIcon? Chevron;
    public Grid? Wrapper;
    public RotateTransform? Rotate;
    public Storyboard? FoldStoryboard;
}
