namespace DevWinUIGallery.Views;

public sealed partial class ContentSliderPage : Page
{
    public List<ContentSliderItem> ContentItems = new List<ContentSliderItem>()
    {
        new ContentSliderItem() {Name="Item 1", Image = "ms-appx:///Assets/BannerView/1.jpeg"},
        new ContentSliderItem() {Name="Item 2", Image = "ms-appx:///Assets/BannerView/2.png"},
        new ContentSliderItem() {Name="Item 3", Image = "ms-appx:///Assets/BannerView/3.jpeg"},
        new ContentSliderItem() {Name="Item 4", Image = "ms-appx:///Assets/BannerView/4.jpeg"},
        new ContentSliderItem() {Name="Item 5", Image = "ms-appx:///Assets/BannerView/5.jpeg"},
        new ContentSliderItem() {Name="Item 6", Image = "ms-appx:///Assets/BannerView/6.png"},
        new ContentSliderItem() {Name="Item 7", Image = "ms-appx:///Assets/BannerView/7.jpeg"},
        new ContentSliderItem() {Name="Item 8", Image = "ms-appx:///Assets/BannerView/8.jpeg"},
        new ContentSliderItem() {Name="Item 9", Image = "ms-appx:///Assets/BannerView/9.jpeg"},
    };
    public ContentSliderPage()
    {
        InitializeComponent();
    }
}
public partial class ContentSliderItem
{
    public string Name { get; set; }
    public string Image { get; set; }

    public override string ToString()
    {
        return Name;
    }
}
