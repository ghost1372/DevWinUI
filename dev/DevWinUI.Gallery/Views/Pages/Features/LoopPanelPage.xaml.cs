namespace DevWinUIGallery.Views;

public sealed partial class LoopPanelPage : Page
{
    public BaseViewModel ViewModel { get; }
    public LoopPanelPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
