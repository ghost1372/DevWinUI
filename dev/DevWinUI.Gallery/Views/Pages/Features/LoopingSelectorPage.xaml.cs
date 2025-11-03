namespace DevWinUIGallery.Views;

public sealed partial class LoopingSelectorPage : Page
{
    public LoopingSelectorPage()
    {
        InitializeComponent();

        LoopingSelectorSample.Items = Enumerable.Range(0, 12).Select(x => new LoopingSelectorItem { PrimaryText = x.ToString() } as object).ToList();
    }
}
