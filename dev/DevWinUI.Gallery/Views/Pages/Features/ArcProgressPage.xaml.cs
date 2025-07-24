namespace DevWinUIGallery.Views;

public sealed partial class ArcProgressPage : Page
{
    public BaseViewModel ViewModel { get;}
    public ArcProgressPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
