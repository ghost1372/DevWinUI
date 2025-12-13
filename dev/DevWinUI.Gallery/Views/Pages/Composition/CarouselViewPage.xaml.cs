namespace DevWinUIGallery.Views;

public sealed partial class CarouselViewPage : Page
{
    public List<ICarouselViewItemSource> Items = new List<ICarouselViewItemSource>()
    {
        new CarouselItemSource() {Title="Item 1", ImageSource = "ms-appx:///Assets/BannerView/1.jpeg"},
        new CarouselItemSource() {Title="Item 2", ImageSource = "ms-appx:///Assets/BannerView/2.png"},
        new CarouselItemSource() {Title="Item 3", ImageSource = "ms-appx:///Assets/BannerView/3.jpeg"},
        new CarouselItemSource() {Title="Item 4", ImageSource = "ms-appx:///Assets/BannerView/4.jpeg"},
        new CarouselItemSource() {Title="Item 5", ImageSource = "ms-appx:///Assets/BannerView/5.jpeg"},
        new CarouselItemSource() {Title="Item 6", ImageSource = "ms-appx:///Assets/BannerView/6.png"},
        new CarouselItemSource() {Title="Item 7", ImageSource = "ms-appx:///Assets/BannerView/7.jpeg"},
        new CarouselItemSource() {Title="Item 8", ImageSource = "ms-appx:///Assets/BannerView/8.jpeg"},
        new CarouselItemSource() {Title="Item 9", ImageSource = "ms-appx:///Assets/BannerView/9.jpeg"},
    };

    public CarouselViewPage()
    {
        InitializeComponent();
    }

    private async void CarouselSample_ItemClick(object sender, CarouselViewItemClickEventArgs e)
    {
        await MessageBox.ShowInfoAsync($"You have clicked {(e.ClickItem as ICarouselViewItemSource).Title} ;-)", "Wow");
    }
}
public class CarouselItemSource : ICarouselViewItemSource
{
    public string ImageSource { get; set; }
    public string Title { get; set; }
}
