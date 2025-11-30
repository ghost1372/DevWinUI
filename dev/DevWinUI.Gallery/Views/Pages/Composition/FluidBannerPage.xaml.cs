namespace DevWinUIGallery.Views;

public sealed partial class FluidBannerPage : Page
{
    public FluidBannerPage()
    {
        InitializeComponent();

        Loaded += FluidBannerPage_Loaded;
    }

    private void FluidBannerPage_Loaded(object sender, RoutedEventArgs e)
    {
        var itemCount = 10;

        var items = new List<Uri>();
        for (var i = 0; i < itemCount; i++)
        {
            items.Add(new Uri(PathHelper.GetFilePath(new Uri($"ms-appx:///Assets/Landscapes/Landscape-{i + 1}.jpg"))));
        }

        Banner.ItemsSource = items;
    }
}
