namespace DevWinUIGallery.Views;

public sealed partial class FluidBannerPage : Page
{
    public BaseViewModel ViewModel { get; }
    public FluidBannerPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();

        Loaded += FluidBannerPage_Loaded;
    }

    private void FluidBannerPage_Loaded(object sender, RoutedEventArgs e)
    {
        Banner.ItemsSource = ViewModel.LandscapeData.Select(x=> PathHelper.GetFilePath(x.ImageUri)).Take(4).ToList();
    }
}
