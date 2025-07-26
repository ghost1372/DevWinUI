namespace DevWinUIGallery.Views;

public sealed partial class DepthLayerViewPage : Page
{
    public BaseViewModel ViewModel { get; }
    public DepthLayerViewPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
