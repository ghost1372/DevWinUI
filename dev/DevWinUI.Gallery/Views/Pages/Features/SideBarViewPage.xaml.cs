namespace DevWinUIGallery.Views;

public sealed partial class SideBarViewPage : Page
{
    public BaseViewModel ViewModel { get; }
    public SideBarViewPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
