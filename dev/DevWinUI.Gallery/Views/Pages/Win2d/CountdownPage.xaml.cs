namespace DevWinUIGallery.Views;

public sealed partial class CountdownPage : Page
{
    public BaseViewModel ViewModel { get; }
    public CountdownPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
