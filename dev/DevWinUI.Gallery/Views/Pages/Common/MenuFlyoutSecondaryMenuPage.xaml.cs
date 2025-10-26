namespace DevWinUIGallery.Views;

public sealed partial class MenuFlyoutSecondaryMenuPage : Page
{
    public BaseViewModel ViewModel { get; }
    public MenuFlyoutSecondaryMenuPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
