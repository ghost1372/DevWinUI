using DevWinUIGallery.Models;

namespace DevWinUIGallery.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<SampleItemModel> LandscapeData { get; set; } = new ObservableCollection<SampleItemModel>()
    {
        new SampleItemModel("Sample 1","Sample Desc 1", "ms-appx:///Assets/Landscapes/Landscape-1.jpg"),
        new SampleItemModel("Sample 2", "Sample Desc 2","ms-appx:///Assets/Landscapes/Landscape-2.jpg"),
        new SampleItemModel("Sample 3","Sample Desc 3", "ms-appx:///Assets/Landscapes/Landscape-3.jpg"),
        new SampleItemModel("Sample 4", "Sample Desc 4","ms-appx:///Assets/Landscapes/Landscape-4.jpg"),
        new SampleItemModel("Sample 5", "Sample Desc 5","ms-appx:///Assets/Landscapes/Landscape-5.jpg"),
        new SampleItemModel("Sample 6", "Sample Desc 6","ms-appx:///Assets/Landscapes/Landscape-6.jpg"),
        new SampleItemModel("Sample 7","Sample Desc 7", "ms-appx:///Assets/Landscapes/Landscape-7.jpg"),
        new SampleItemModel("Sample 8", "Sample Desc 8","ms-appx:///Assets/Landscapes/Landscape-8.jpg"),
        new SampleItemModel("Sample 9", "Sample Desc 9","ms-appx:///Assets/Landscapes/Landscape-9.jpg"),
        new SampleItemModel("Sample 10", "Sample Desc 10","ms-appx:///Assets/Landscapes/Landscape-10.jpg"),
        new SampleItemModel("Sample 11", "Sample Desc 11","ms-appx:///Assets/Landscapes/Landscape-11.jpg"),
        new SampleItemModel("Sample 12", "Sample Desc 12","ms-appx:///Assets/Landscapes/Landscape-12.jpg"),
        new SampleItemModel("Sample 13", "Sample Desc 13","ms-appx:///Assets/Landscapes/Landscape-13.jpg")
    };

    [ObservableProperty]
    public partial ObservableCollection<SampleItemModel> BannerViewData { get; set; } = new ObservableCollection<SampleItemModel>()
    {
        new SampleItemModel("Sample 1","Sample Desc 1", "ms-appx:///Assets/BannerView/1.jpeg"),
        new SampleItemModel("Sample 2", "Sample Desc 2","ms-appx:///Assets/BannerView/2.png"),
        new SampleItemModel("Sample 3","Sample Desc 3", "ms-appx:///Assets/BannerView/3.jpeg"),
        new SampleItemModel("Sample 4", "Sample Desc 4","ms-appx:///Assets/BannerView/4.jpeg"),
        new SampleItemModel("Sample 5", "Sample Desc 5","ms-appx:///Assets/BannerView/5.jpeg"),
        new SampleItemModel("Sample 6", "Sample Desc 6","ms-appx:///Assets/BannerView/6.png"),
        new SampleItemModel("Sample 7","Sample Desc 7", "ms-appx:///Assets/BannerView/7.jpeg"),
        new SampleItemModel("Sample 8", "Sample Desc 8","ms-appx:///Assets/BannerView/8.jpeg"),
        new SampleItemModel("Sample 9", "Sample Desc 9","ms-appx:///Assets/BannerView/9.jpeg")
    };

    [ObservableProperty]
    public partial ObservableCollection<SampleItemModel> CarouselData { get; set; } = new ObservableCollection<SampleItemModel>()
    {
        new SampleItemModel("Wolverine & Deadpool","Action-packed mutant duo", "ms-appx:///Assets/Carousel/1.jpg"),
        new SampleItemModel("Alien", "Survival horror in space","ms-appx:///Assets/Carousel/2.jpg"),
        new SampleItemModel("Walking Dead: Dead City","Zombies overrun New York", "ms-appx:///Assets/Carousel/3.jpg"),
        new SampleItemModel("Inferno", "Thrilling race against time","ms-appx:///Assets/Carousel/4.jpg"),
        new SampleItemModel("The Forest Song", "Magical tale of nature and love","ms-appx:///Assets/Carousel/5.jpg"),
        new SampleItemModel("Moana", "Epic ocean adventure","ms-appx:///Assets/Carousel/6.jpg"),
        new SampleItemModel("Avatar","Journey to Pandora", "ms-appx:///Assets/Carousel/7.jpg"),
        new SampleItemModel("Wish", "A magical wish adventure","ms-appx:///Assets/Carousel/8.jpg"),
        new SampleItemModel("Extraction", "A dangerous rescue mission","ms-appx:///Assets/Carousel/9.jpg"),
        new SampleItemModel("Mandalorian", "A lone bounty hunter’s journey","ms-appx:///Assets/Carousel/10.jpg")
    };

    [ObservableProperty]
    public partial ObservableCollection<SampleItemModel> ProfileData { get; set; } = new ObservableCollection<SampleItemModel>()
    {
        new SampleItemModel("ms-appx:///Assets/Profile/1.jpg"),
        new SampleItemModel("ms-appx:///Assets/Profile/2.jpg"),
        new SampleItemModel("ms-appx:///Assets/Profile/3.jpg"),
        new SampleItemModel("ms-appx:///Assets/Profile/4.jpg"),
        new SampleItemModel("ms-appx:///Assets/Profile/5.jpg"),
        new SampleItemModel("ms-appx:///Assets/Profile/6.jpg"),
    };

    [ObservableProperty]
    public partial List<ICarouselViewItemSource> CarouselViewData { get; set; } = new List<ICarouselViewItemSource>();

    [ObservableProperty]
    public partial ObservableCollection<StoreCarouselItem> StoreCarouselData { get; set; } = new ObservableCollection<StoreCarouselItem>();

    [ObservableProperty]
    public partial ObservableCollection<SampleItemModel> DepthLayerData { get; set; } = new ObservableCollection<SampleItemModel>();

    [ObservableProperty]
    public partial DigitalSegmentOption DigitalSegmentSelectedModel { get; set; }

    [ObservableProperty]
    public partial RadioButton SwitchPresenterRadioSelectedItem { get; set; }

    [ObservableProperty]
    public partial Uri AnimatedImageImagePath { get; set; }
    internal void GenerateDepthLayerData()
    {
        DepthLayerData = new(LandscapeData.OrderBy(x => x.IntValue).ToList());
    }
    internal void GenerateStoreCarouselData()
    {
        StoreCarouselData = new(CarouselData.Select(x => new StoreCarouselItem
        {
            Title = x.Title,
            Description = x.Description,
            ImageSource = PathHelper.GetFilePath(new Uri(x.ImagePath)).OriginalString
        }).Cast<StoreCarouselItem>().ToList());
    }
    internal void GenerateCarouselViewData()
    {
        CarouselViewData = BannerViewData.Select(x => new CarouselItemSource
        {
            Title = x.Title,
            ImageSource = x.ImagePath
        }).Cast<ICarouselViewItemSource>().ToList();
    }
}
