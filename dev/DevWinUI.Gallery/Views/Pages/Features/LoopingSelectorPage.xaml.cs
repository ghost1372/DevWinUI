namespace DevWinUIGallery.Views;

public sealed partial class LoopingSelectorPage : Page
{
    public LoopingSelectorPage()
    {
        InitializeComponent();

        LoopingSelectorSample.Items = Enumerable.Range(0, 12).Cast<object>().ToList();
    }
}
