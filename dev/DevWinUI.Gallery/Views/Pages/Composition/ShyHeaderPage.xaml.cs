namespace DevWinUIGallery.Views;

public sealed partial class ShyHeaderPage : Page
{
    public BaseViewModel ViewModel { get;}
    public ShyHeaderPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
