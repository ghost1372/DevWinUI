namespace DevWinUIGallery.Views;

public sealed partial class EasyCarouselPanelPage : Page
{
    public List<EasyCarouselPanelItem> CarouselItems = new List<EasyCarouselPanelItem>()
    {
        new EasyCarouselPanelItem() {Name="Item 1", Image = "ms-appx:///Assets/BannerView/1.jpeg"},
        new EasyCarouselPanelItem() {Name="Item 2", Image = "ms-appx:///Assets/BannerView/2.png"},
        new EasyCarouselPanelItem() {Name="Item 3", Image = "ms-appx:///Assets/BannerView/3.jpeg"},
        new EasyCarouselPanelItem() {Name="Item 4", Image = "ms-appx:///Assets/BannerView/4.jpeg"},
        new EasyCarouselPanelItem() {Name="Item 5", Image = "ms-appx:///Assets/BannerView/5.jpeg"},
        new EasyCarouselPanelItem() {Name="Item 6", Image = "ms-appx:///Assets/BannerView/6.png"},
        new EasyCarouselPanelItem() {Name="Item 7", Image = "ms-appx:///Assets/BannerView/7.jpeg"},
        new EasyCarouselPanelItem() {Name="Item 8", Image = "ms-appx:///Assets/BannerView/8.jpeg"},
        new EasyCarouselPanelItem() {Name="Item 9", Image = "ms-appx:///Assets/BannerView/9.jpeg"},
    };

    public EasyCarouselPanelPage()
    {
        InitializeComponent();
    }

    private async void EasyCarouselPanelSample_ItemTapped(object sender, FrameworkElement e)
    {
        await MessageBox.ShowInfoAsync((e?.DataContext as EasyCarouselPanelItem)?.Name);
    }
}
public partial class EasyCarouselPanelItem
{
    public string Name { get; set; }
    public string Image { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
