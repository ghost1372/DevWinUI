namespace DevWinUIGallery.Views;

public sealed partial class FlipCardsPage : Page
{
    public BaseViewModel ViewModel { get; }
    public FlipCardsPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
