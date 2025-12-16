namespace DevWinUIGallery.Views;

public sealed partial class MenuFlyoutSecondaryMenuPage : Page
{
    public ObservableCollection<MenuFlyoutSecondaryMenuPlacement> SecondaryMenuPlacement { get; set; } = new ObservableCollection<MenuFlyoutSecondaryMenuPlacement>(Enum.GetValues<MenuFlyoutSecondaryMenuPlacement>());
    public MenuFlyoutSecondaryMenuPage()
    {
        InitializeComponent();
    }
}
