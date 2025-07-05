namespace DevWinUIGallery.Views;

public sealed partial class AnimatedImagePage : Page
{
    public BaseViewModel ViewModel { get; }
    public AnimatedImagePage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.InitAnimatedImageTimer();
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.StopTimer();
    }
}
