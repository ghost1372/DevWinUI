namespace DevWinUIGallery.Views;

public sealed partial class CoverFlowPage : Page
{
    public BaseViewModel ViewModel { get; }
    public CoverFlowPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
