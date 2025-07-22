namespace DevWinUIGallery.Views;

public sealed partial class SplitCirclePage : Page
{
    public BaseViewModel ViewModel { get; }
    public SplitCirclePage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
